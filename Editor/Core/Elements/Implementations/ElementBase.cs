using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolkit.Core;
using EasyToolkit.Core.Diagnostics;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Provides a base implementation of the <see cref="IElement"/> interface,
    /// serving as the foundation for all inspector element implementations.
    /// </summary>
    public abstract class ElementBase : IElement, IDisposable
    {
        [CanBeNull] private IElementList<IElement> _children;
        private int? _lastUpdateId;
        private GUIContent _label;
        private ElementPhases _phases;
        private bool _isFirstRefreshed;
        [CanBeNull] private VisualElement _visualElement;

        [CanBeNull] private IAttributeResolver _attributeResolver;
        [CanBeNull] private IDrawerChainResolver _drawerChainResolver;
        [CanBeNull] private IPostProcessorChainResolver _postProcessorChainResolver;
        [CanBeNull] private IVisualBuilderResolver _visualBuilderResolver;
        [CanBeNull] private IVisualProcessorChainResolver _visualProcessorChainResolver;
        [CanBeNull] private IMessageDispatcher _messageDispatcher;

        /// <summary>
        /// Gets the definition that describes this element.
        /// </summary>
        public IElementDefinition Definition { get; }

        /// <summary>
        /// Gets the element shared context that provides access to tree-level services and resolver factories.
        /// </summary>
        public IElementSharedContext SharedContext { get; }

        /// <summary>
        /// Gets the current parent element in the element tree hierarchy.
        /// </summary>
        public IElement Parent { get; protected set; }

        /// <summary>
        /// Gets the runtime state of this element.
        /// </summary>
        public IElementState State { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBase"/> class.
        /// </summary>
        /// <param name="definition">The definition that describes this element.</param>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        protected ElementBase(
            [NotNull] IElementDefinition definition,
            [NotNull] IElementSharedContext sharedContext)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            SharedContext = sharedContext ?? throw new ArgumentNullException(nameof(sharedContext));

            State = new ElementState(this);
        }

        /// <summary>
        /// Gets the hierarchical path of this element.
        /// </summary>
        public abstract string Path { get; }

        /// <summary>
        /// Gets or sets the label displayed in the inspector.
        /// </summary>
        public GUIContent Label
        {
            get
            {
                if (_label == null)
                {
                    var displayName = ObjectNames.NicifyVariableName(Definition.Name);
                    _label = new GUIContent(displayName);
                }

                return _label;
            }
            set => _label = value;
        }

        /// <summary>
        /// Gets child elements that were added or removed at runtime.
        /// This collection is mutable and reflects runtime modifications.
        /// </summary>
        public IElementList<IElement> Children
        {
            get
            {
                ValidateDisposed();
                return _children;
            }
        }

        public VisualElement VisualElement
        {
            get
            {
                ValidateDisposed();
                return _visualElement;
            }
        }

        public VisualElement SpecificOwningVisualElement { get; set; }

        /// <summary>
        /// Gets the current phase of this element.
        /// </summary>
        public ElementPhases Phases => _phases;

        /// <summary>
        /// Gets all custom attribute infos applied to this element.
        /// </summary>
        /// <returns>An array of element attribute infos.</returns>
        public IReadOnlyList<ElementAttributeInfo> GetAttributeInfos()
        {
            ValidateDisposed();
            if (_attributeResolver == null)
            {
                return Array.Empty<ElementAttributeInfo>();
            }

            return _attributeResolver.GetAttributeInfos();
        }

        /// <summary>
        /// Tries to get the attribute info for the specified attribute instance.
        /// </summary>
        /// <param name="attribute">The attribute to find.</param>
        /// <param name="attributeInfo">When this method returns, contains the attribute info if found; otherwise, null.</param>
        /// <returns>true if the attribute info was found; otherwise, false.</returns>
        public bool TryGetAttributeInfo(Attribute attribute, out ElementAttributeInfo attributeInfo)
        {
            ValidateDisposed();
            if (_attributeResolver == null)
            {
                attributeInfo = null;
                return false;
            }

            return _attributeResolver.TryGetAttributeInfo(attribute, out attributeInfo);
        }

        /// <summary>
        /// Gets the drawer chain for rendering this element.
        /// </summary>
        /// <returns>The drawer chain containing all applicable drawers.</returns>
        private DrawerChain GetDrawerChain()
        {
            ValidateDisposed();
            if (_drawerChainResolver == null)
            {
                return new DrawerChain(this, Array.Empty<IEasyDrawer>());
            }

            return _drawerChainResolver.GetDrawerChain();
        }

        private PostProcessorChain GetPostProcessorChain()
        {
            ValidateDisposed();
            if (_postProcessorChainResolver == null)
            {
                return new PostProcessorChain(this, Array.Empty<IPostProcessor>());
            }

            return _postProcessorChainResolver.GetPostProcessorChain();
        }

        public virtual bool Request(Action action, bool forceDelay = false)
        {
            if (_phases.IsDrawing())
            {
                SharedContext.Tree.QueueCallback(action);
            }
            else
            {
                action();
            }

            return true;
        }

        /// <summary>
        /// Gets the message dispatcher for this element (lazy initialization).
        /// </summary>
        [NotNull] private IMessageDispatcher MessageDispatcher
        {
            get
            {
                if (_messageDispatcher == null)
                {
                    _messageDispatcher = MessageDispatcherFactory.Create(this);
                }

                return _messageDispatcher;
            }
        }

        /// <inheritdoc/>
        public bool Send(string messageName, object args = null)
        {
            ValidateDisposed();
            return MessageDispatcher.Dispatch(messageName, args);
        }

        /// <inheritdoc/>
        public TResult Send<TResult>(string messageName, object args = null)
        {
            ValidateDisposed();

            if (_messageDispatcher is IMessageDispatcher<TResult> typedDispatcher)
            {
                return typedDispatcher.Dispatch(messageName, args);
            }

            return default;
        }

        /// <summary>
        /// Draws this element in the inspector with the specified label.
        /// </summary>
        /// <param name="label">The label to display. If null, uses the element's default label.</param>
        public virtual void Draw(GUIContent label)
        {
            ValidateDisposed();

            ((IElement)this).Update();

            switch (SharedContext.Tree.BackendMode)
            {
                case InspectorBackendMode.IMGUI:
                {
                    var chain = GetDrawerChain();
                    chain.Reset();

                    if (chain.MoveNext() && chain.Current != null)
                    {
                        _phases = _phases.Add(ElementPhases.Drawing);
                        chain.Current.Draw(label);
                        _phases = _phases.Remove(ElementPhases.Drawing);
                    }

                    break;
                }
                case InspectorBackendMode.UIToolkit:
                {
                    if (_phases.IsPendingDraw())
                    {
                        _phases = _phases.Add(ElementPhases.Drawing);

                        var owningVisualElement = GetOwningVisualElement();
                        Assert.IsTrue(owningVisualElement != null);
                        int originalIndex = -1;
                        if (_visualElement != null)
                        {
                            originalIndex = owningVisualElement.IndexOf(_visualElement);
                            if (originalIndex == -1)
                            {
                                Debug.LogError(
                                    $"VisualElementNotFound: Existing visual element not found in root hierarchy (Path: {Path}, VisualElement: {_visualElement.GetType()})");
                            }

                            owningVisualElement.RemoveAt(originalIndex);
                        }

                        _visualElement = CreateVisualElement();
                        if (_visualElement != null)
                        {
                            if (originalIndex != -1)
                            {
                                owningVisualElement.Insert(originalIndex, _visualElement);
                            }
                            else
                            {
                                owningVisualElement.Add(_visualElement);
                            }
                        }

                        _phases = _phases.Remove(ElementPhases.Drawing);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            _phases = _phases.Remove(ElementPhases.PendingDraw);
        }

        /// <summary>
        /// Refreshes all element state, forcing recreation of resolvers and children.
        /// </summary>
        public bool RequestRefresh()
        {
            ValidateDisposed();

            if (_isFirstRefreshed && !IsNecessaryToRefreshMultiple())
            {
                return false;
            }

            _phases = _phases.Add(ElementPhases.PendingRefresh);
            if (Request(Refresh))
            {
                return true;
            }

            _phases = _phases.Remove(ElementPhases.PendingRefresh);
            return false;
        }

        /// <summary>
        /// Destroys this element, disposing it and removing it from the factory's tracking container.
        /// If the element is not in an idle state, the destruction is queued and executed later.
        /// </summary>
        public void Destroy()
        {
            if (_phases.IsDestroyed() || _phases.IsPendingDestroy() || _phases.IsDestroying())
            {
                return;
            }

            _phases = _phases.Add(ElementPhases.PendingDestroy);
            SharedContext.Tree.ElementFactory.DestroyElement(this);
        }

        protected virtual void OnUpdate(bool forceUpdate)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected virtual void Dispose()
        {
            if (Parent != null)
            {
                Parent.Children.TryRemove(this);
            }

            if (_children != null)
            {
                _children.BeforeElementMoved -= OnChildrenElementMoved;
                _children.AfterElementMoved -= OnChildrenElementMoved;
            }

            (_children as IDisposable)?.Dispose();

            // Release resolvers back to pool
            if (_attributeResolver != null)
            {
                ResolverUtility.ReleaseResolver(_attributeResolver);
                _attributeResolver = null;
            }

            if (_drawerChainResolver != null)
            {
                ResolverUtility.ReleaseResolver(_drawerChainResolver);
                _drawerChainResolver = null;
            }

            if (_postProcessorChainResolver != null)
            {
                ResolverUtility.ReleaseResolver(_postProcessorChainResolver);
                _postProcessorChainResolver = null;
            }

            if (_visualBuilderResolver != null)
            {
                ResolverUtility.ReleaseResolver(_visualBuilderResolver);
                _visualBuilderResolver = null;
            }

            if (_visualProcessorChainResolver != null)
            {
                ResolverUtility.ReleaseResolver(_visualProcessorChainResolver);
                _visualProcessorChainResolver = null;
            }
        }

        [NotNull]
        protected virtual IElementList<IElement> CreateChildren()
        {
            var children = new RequestedElementList<IElement>(this);
            return children;
        }

        /// <summary>
        /// Handles the <see cref="ElementList{TElement}.AfterElementMoved"/> event from children collection.
        /// Triggers the event through the shared context to notify the entire element tree.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event arguments containing element move details.</param>
        private void OnChildrenElementMoved(object sender, ElementMovedEventArgs args)
        {
            SharedContext.TriggerEvent(this, args);
        }

        /// <summary>
        /// Handles element moved events by updating parent-child relationships.
        /// This method is called directly by <see cref="ElementList{TElement}"/> when elements are inserted or removed,
        /// rather than through global event broadcasting to avoid O(n²) performance degradation.
        /// </summary>
        /// <param name="args">The event arguments containing element move details.</param>
        [MessageHandler(ElementMessageNames.ElementMoved)]
        private void OnElementMoved(ElementMovedEventArgs args)
        {
            if (args.Timing == ElementMovedTiming.After)
            {
                // NOTE: Checking args.NewParent != null is necessary for the following reason:
                // NewParent being null typically occurs when an element is removed from its original Children collection.
                // Moving an element from one Children to another is a two-step process:
                //   1) Remove from the original Children (triggers TryRemove)
                //   2) Add to the new Children
                // The execution order of these two steps is uncertain. It's possible that the element is added to
                // the new Children first, and then removed from the original Children.
                // If removal from the original Children occurs after being added to the new Children, it will
                // trigger ElementMovedEventArgs again with NewParent as null. Without checking NewParent != null,
                // this would incorrectly cause the element's Parent to be set to null.
                if (args.Element == this && args.NewParent != null)
                {
                    Request(() => Parent = args.NewParent);
                    return;
                }

                if (args.ChangeType == ElementListChangeType.Insert && args.NewParent != this)
                {
                    // Element was moved to another parent but still exists in our children list
                    // This indicates the element needs to be updated/removed
                    _children?.TryRemove(args.Element);
                }
            }
        }

        protected void ValidateDisposed()
        {
            if (_phases.IsDestroyed())
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public override string ToString()
        {
            var typeName = GetType().Name;
            var suffix = "Element";
            return $"{Path}:{typeName.Substring(0, typeName.Length - suffix.Length)}";
        }


        protected virtual bool CanHaveChildren()
        {
            return false;
        }

        protected virtual void Refresh()
        {
            _phases = _phases.Remove(ElementPhases.PendingRefresh);
            _phases = _phases.Add(ElementPhases.Refreshing);

            (_children as IDisposable)?.Dispose();
            if (CanHaveChildren())
            {
                // Recreate children list
                _children = CreateChildren();
                _children.BeforeElementMoved += OnChildrenElementMoved;
                _children.AfterElementMoved += OnChildrenElementMoved;
            }
            else
            {
                _children = null;
            }

            // Release old attribute resolver before creating new one
            if (_attributeResolver != null)
            {
                ResolverUtility.ReleaseResolver(_attributeResolver);
                _attributeResolver = null;
            }

            _attributeResolver = SharedContext.GetResolverFactory<IAttributeResolver>()
                .CreateResolver(this);

            // Release old post processor chain resolver before creating new one
            if (_postProcessorChainResolver != null)
            {
                ResolverUtility.ReleaseResolver(_postProcessorChainResolver);
                _postProcessorChainResolver = null;
            }

            _postProcessorChainResolver = SharedContext.GetResolverFactory<IPostProcessorChainResolver>()
                .CreateResolver(this);

            switch (SharedContext.Tree.BackendMode)
            {
                case InspectorBackendMode.UIToolkit:
                {
                    // Release old visual builder resolver before creating new one
                    if (_visualBuilderResolver != null)
                    {
                        ResolverUtility.ReleaseResolver(_visualBuilderResolver);
                        _visualBuilderResolver = null;
                    }

                    _visualBuilderResolver = SharedContext.GetResolverFactory<IVisualBuilderResolver>()
                        .CreateResolver(this);

                    // Release old visual processor chain resolver before creating new one
                    if (_visualProcessorChainResolver != null)
                    {
                        ResolverUtility.ReleaseResolver(_visualProcessorChainResolver);
                        _visualProcessorChainResolver = null;
                    }

                    _visualProcessorChainResolver = SharedContext.GetResolverFactory<IVisualProcessorChainResolver>()
                        .CreateResolver(this);

                    break;
                }
                case InspectorBackendMode.IMGUI:
                {
                    // Release old drawer chain resolver before creating new one
                    if (_drawerChainResolver != null)
                    {
                        ResolverUtility.ReleaseResolver(_drawerChainResolver);
                        _drawerChainResolver = null;
                    }

                    _drawerChainResolver = SharedContext.GetResolverFactory<IDrawerChainResolver>()
                        .CreateResolver(this);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _phases = _phases.Remove(ElementPhases.Refreshing);
            _phases = _phases.Add(ElementPhases.JustRefreshed);
            _phases = _phases.Add(ElementPhases.PendingPostProcess);
            _phases = _phases.Add(ElementPhases.PendingDraw);
            _isFirstRefreshed = true;
        }

        public virtual bool PostProcessIfNeeded()
        {
            if (_phases.IsPendingPostProcess())
            {
                var chain = GetPostProcessorChain();
                chain.Reset();
                while (chain.MoveNext() && chain.Current != null)
                {
                    _phases = _phases.Add(ElementPhases.PostProcessing);
                    chain.Current.Process();
                    _phases = _phases.Remove(ElementPhases.PostProcessing);
                }

                _phases = _phases.Remove(ElementPhases.PendingPostProcess);
                return true;
            }

            return false;
        }

        public void Update(bool forceUpdate)
        {
            if (_lastUpdateId == SharedContext.UpdateId && !forceUpdate)
            {
                return;
            }

            if (_phases.IsJustRefreshed())
            {
                _phases = _phases.Remove(ElementPhases.JustRefreshed);
            }

            if (_lastUpdateId == null)
            {
                RequestRefresh();
            }

            _lastUpdateId = SharedContext.UpdateId;
            OnUpdate(forceUpdate);
        }

        protected virtual VisualElement CreateVisualElement()
        {
            var visualBuilder = _visualBuilderResolver!.GetVisualBuilder();
            if (visualBuilder == null)
            {
                var elementType = GetType();

                if (this is IValueElement valueElement)
                {
                    var valueType = valueElement.ValueEntry.ValueType;
                    Debug.LogError(
                        $"VisualBuilderNotFound: No VisualBuilder defined for element type '{elementType}' (ValueType: {valueType}). " +
                        $"The visual builder resolver could not find a visual builder to create the UI representation. " +
                        $"To fix this, define a VisualValueBuilder<{valueType}> for the value type.");
                }
                else if (this is IMethodElement methodElement)
                {
                    Debug.LogError(
                        $"VisualBuilderNotFound: No VisualBuilder defined for element type '{elementType}'." +
                        $"The visual builder resolver could not find a visual builder to create the UI representation. " +
                        $"To fix this, define a VisualMethodBuilder<TMethodAttribute> for the method '{methodElement.Definition.MethodInfo}'.");
                }
                else if (this is IGroupElement groupElement)
                {
                    Debug.LogError(
                        $"VisualBuilderNotFound: No VisualBuilder defined for element type '{elementType}'." +
                        $"The visual builder resolver could not find a visual builder to create the UI representation. " +
                        $"To fix this, define a VisualGroupBuilder<TGroupAttribute> for the group '{groupElement.Definition.BeginGroupAttributeType}' and '{groupElement.Definition.EndGroupAttributeType}'.");
                }
                else
                {
                    Debug.LogError(
                        $"VisualBuilderNotFound: No VisualBuilder defined for element type '{elementType}'. " +
                        $"The visual builder resolver could not find a visual builder to create the UI representation. " +
                        $"To fix this, define a VisualBuilder for the element type.");
                }

                return null;
            }

            visualBuilder.Element = this;
            var visualElement = visualBuilder.CreateVisualElement();
            if (visualElement == null)
            {
                return null;
            }

            var chain = _visualProcessorChainResolver!.GetVisualProcessorChain();
            chain.Reset();

            if (chain.MoveNext() && chain.Current != null)
            {
                chain.Current.Process(visualElement);
            }

            return visualElement;
        }

        protected virtual bool IsNecessaryToRefreshMultiple()
        {
            return true;
        }

        protected virtual VisualElement GetOwningVisualElement()
        {
            if (SharedContext.Tree.BackendMode != InspectorBackendMode.UIToolkit)
            {
                return null;
            }

            if (SpecificOwningVisualElement != null)
            {
                return SpecificOwningVisualElement;
            }

            if (this is IRootElement)
            {
                return SharedContext.Tree.RootVisualElement;
            }

            IElement current = this;
            do
            {
                current = current.Parent;
                if (current == null)
                {
                    return null;
                }

                if (current.VisualElement != null)
                {
                    return current.VisualElement;
                }

                if (current is IRootElement)
                {
                    return SharedContext.Tree.RootVisualElement;
                }

            } while (false);

            return null;
        }

        void IDisposable.Dispose()
        {
            if (_phases.IsDestroyed() || _phases.IsDestroying())
            {
                return;
            }

            _phases = _phases.Remove(ElementPhases.PendingDestroy);
            _phases = _phases.Add(ElementPhases.Destroying);
            Dispose();
            _phases = _phases.Add(ElementPhases.Destroyed);
        }
    }
}
