using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    public class ElementCreator
    {
        private const string ErrorDefinitionNull = "Element definition cannot be null.";
        private readonly ElementSharedContext _sharedContext;
        private readonly HashSet<IElement> _createdElements;
        private readonly HashSet<IElement> _pendingDestroyElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementCreator"/> class.
        /// </summary>
        /// <param name="sharedContext">The shared context providing access to tree-level services.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sharedContext"/> is null.</exception>
        public ElementCreator([NotNull] ElementSharedContext sharedContext)
        {
            _sharedContext = sharedContext ?? throw new ArgumentNullException(nameof(sharedContext));
            _createdElements = new HashSet<IElement>();
            _pendingDestroyElements = new HashSet<IElement>();
        }

        /// <summary>
        /// Creates an element that matches the concrete type of the supplied definition.
        /// </summary>
        /// <param name="definition">The definition that describes the element to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new element instance matching <paramref name="definition"/>.</returns>
        [NotNull]
        public IElement CreateElement(
            [NotNull] IElementDefinition definition,
            [CanBeNull] ILogicalElement parent)
        {
            if (definition is IFieldCollectionDefinition fieldCollectionDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return CreateFieldCollectionElement(fieldCollectionDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for field collection definition", nameof(parent));
            }

            if (definition is IPropertyCollectionDefinition propertyCollectionDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return CreatePropertyCollectionElement(propertyCollectionDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for property collection definition", nameof(parent));
            }

            if (definition is IFieldDefinition fieldDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return CreateFieldElement(fieldDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for field definition", nameof(parent));
            }

            if (definition is IPropertyDefinition propertyDefinition)
            {
                if (parent is IValueElement || parent is null)
                {
                    return CreatePropertyElement(propertyDefinition, (IValueElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a value element for property definition", nameof(parent));
            }

            if (definition is ICollectionItemDefinition collectionItemDefinition)
            {
                if (parent is ICollectionElement || parent is null)
                {
                    return CreateCollectionItemElement(collectionItemDefinition, (ICollectionElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a collection element for collection item definition", nameof(parent));
            }

            if (definition is IMethodParameterDefinition methodParameterDefinition)
            {
                if (parent is IMethodElement || parent is null)
                {
                    return CreateMethodParameterElement(methodParameterDefinition, (IMethodElement)parent);
                }
                throw new ArgumentException($"Parent '{parent}' must be a method element for method parameter definition", nameof(parent));
            }

            if (definition is IRootDefinition rootDefinition)
            {
                return CreateRootElement(rootDefinition);
            }

            if (definition is IMethodDefinition methodDefinition)
            {
                return CreateMethodElement(methodDefinition, parent);
            }

            if (definition is IGroupDefinition groupDefinition)
            {
                return CreateGroupElement(groupDefinition);
            }

            if (definition is ICollectionDefinition collectionDefinition)
            {
                return CreateCollectionElement(collectionDefinition, parent);
            }

            if (definition is IValueDefinition valueDefinition)
            {
                return CreateValueElement(valueDefinition);
            }

            throw new ArgumentException($"Definition '{definition}' is not a valid element definition", nameof(definition));
        }

        /// <summary>
        /// Creates a value element from the given value definition.
        /// </summary>
        /// <param name="definition">The value definition describing the value element to create.</param>
        /// <returns>A new value element instance.</returns>
        [NotNull]
        public IValueElement CreateValueElement([NotNull] IValueDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.ValueElement(definition, _sharedContext, null));
        }

        /// <summary>
        /// Creates a field element from the given field definition.
        /// </summary>
        /// <param name="definition">The field definition describing the field element to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new field element instance.</returns>
        [NotNull]
        public IFieldElement CreateFieldElement([NotNull] IFieldDefinition definition, [CanBeNull] IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.FieldElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a property element from the given property definition.
        /// </summary>
        /// <param name="definition">The property definition describing the property element to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new property element instance.</returns>
        [NotNull]
        public IPropertyElement CreatePropertyElement([NotNull] IPropertyDefinition definition, [CanBeNull] IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.PropertyElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a group element from the given group definition.
        /// </summary>
        /// <param name="definition">The group definition describing the group to create.</param>
        /// <returns>A new group element instance.</returns>
        [NotNull]
        public IGroupElement CreateGroupElement([NotNull] IGroupDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.GroupElement(definition, _sharedContext));
        }

        /// <summary>
        /// Creates a method element from the given method definition.
        /// </summary>
        /// <param name="definition">The method definition describing the method to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new method element instance.</returns>
        [NotNull]
        public IMethodElement CreateMethodElement([NotNull] IMethodDefinition definition, [CanBeNull] ILogicalElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.MethodElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a method parameter element from the given method parameter definition.
        /// </summary>
        /// <param name="definition">The method parameter definition describing the parameter element to create.</param>
        /// <param name="parent">The optional logical parent method element that contains this parameter.</param>
        /// <returns>A new method parameter element instance.</returns>
        [NotNull]
        public IMethodParameterElement CreateMethodParameterElement(
            [NotNull] IMethodParameterDefinition definition,
            [CanBeNull] IMethodElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.MethodParameterElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a collection element from the given collection definition.
        /// </summary>
        /// <param name="definition">The collection definition describing the collection to create.</param>
        /// <param name="parent">The optional logical parent element in the code structure.</param>
        /// <returns>A new collection element instance.</returns>
        [NotNull]
        public ICollectionElement CreateCollectionElement(
            [NotNull] ICollectionDefinition definition,
            [CanBeNull] ILogicalElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.CollectionElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a property collection element from the given property collection definition.
        /// </summary>
        /// <param name="definition">The property collection definition describing the property collection to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new property collection element instance.</returns>
        [NotNull]
        public IPropertyCollectionElement CreatePropertyCollectionElement(
            [NotNull] IPropertyCollectionDefinition definition,
            [CanBeNull] IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.PropertyCollectionElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a field collection element from the given field collection definition.
        /// </summary>
        /// <param name="definition">The field collection definition describing the field collection to create.</param>
        /// <param name="parent">The optional logical parent value element in the code structure.</param>
        /// <returns>A new field collection element instance.</returns>
        [NotNull]
        public IFieldCollectionElement CreateFieldCollectionElement(
            [NotNull] IFieldCollectionDefinition definition,
            [CanBeNull] IValueElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.FieldCollectionElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a collection item element from the given collection item definition.
        /// </summary>
        /// <param name="definition">The collection item definition describing the collection item to create.</param>
        /// <param name="parent">The optional logical parent collection element that contains this item.</param>
        /// <returns>A new collection item element instance.</returns>
        [NotNull]
        public ICollectionItemElement CreateCollectionItemElement(
            [NotNull] ICollectionItemDefinition definition,
            [CanBeNull] ICollectionElement parent)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.CollectionItemElement(definition, _sharedContext, parent));
        }

        /// <summary>
        /// Creates a root element from the given root definition.
        /// </summary>
        /// <param name="definition">The root definition describing the root element to create.</param>
        /// <returns>A new root element instance.</returns>
        [NotNull]
        public IRootElement CreateRootElement([NotNull] IRootDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition), ErrorDefinitionNull);

            return RegisterElement(new Implementations.RootElement(definition, _sharedContext));
        }

        /// <summary>
        /// Destroys the specified element, disposing it and removing it from the factory's tracking container.
        /// If the element is not in an idle state, the destruction is queued and executed later.
        /// </summary>
        /// <param name="element">The element to destroy.</param>
        /// <returns><c>true</c> if the element was successfully destroyed or queued for destruction; <c>false</c> if the element was not found in the tracking container or is already pending destruction.</returns>
        public bool DestroyElement([NotNull] IElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!_createdElements.Contains(element))
                return false;

            // Check if already pending destruction to prevent duplicate queued destroys
            if (_pendingDestroyElements.Contains(element))
                return false;

            if (element.Phases.IsNone() || element.Phases.IsPendingDestroy())
            {
                PerformDestroy(element);
            }
            else
            {
                _pendingDestroyElements.Add(element);
                _sharedContext.Tree.QueueCallback(() => PerformDestroy(element));
            }

            return true;
        }

        /// <summary>
        /// Performs the actual destruction of the specified element.
        /// </summary>
        /// <param name="element">The element to destroy.</param>
        private void PerformDestroy(IElement element)
        {
            // Trigger destroy event before disposal
            using var eventArgs = ElementDestroyedEventArgs.Create(element);
            _sharedContext.TriggerEvent(this, eventArgs);

            // Dispose the element
            (element as IDisposable)?.Dispose();
            // Remove from tracking
            _createdElements.Remove(element);
            // Remove from pending destruction set
            _pendingDestroyElements.Remove(element);
        }

        /// <summary>
        /// Registers the specified element to the factory's tracking container.
        /// </summary>
        /// <typeparam name="T">The type of element that implements <see cref="IElement"/>.</typeparam>
        /// <param name="element">The element to register.</param>
        /// <returns>The same element instance after registration.</returns>
        private T RegisterElement<T>([NotNull] T element) where T : IElement
        {
            _createdElements.Add(element);
            return element;
        }
    }
}
