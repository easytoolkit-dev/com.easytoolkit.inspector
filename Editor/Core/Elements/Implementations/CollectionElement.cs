using System;
using System.Collections.Generic;
using EasyToolkit.Core.Reflection;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    public class CollectionElement : ValueElement, ICollectionElement
    {
        private ElementList<ILogicalElement> _mutableLogicalChildren;

        public CollectionElement(
            [NotNull] IValueDefinition definition,
            [NotNull] IElementSharedContext sharedContext,
            [CanBeNull] ILogicalElement logicalParent)
            : base(definition, sharedContext, logicalParent)
        {
        }

        public ICollectionDefinition Definition => (ICollectionDefinition)base.Definition;

        public IReadOnlyElementList<ICollectionItemElement> LogicalChildren =>
            ((IReadOnlyElementListBoxedWrapper<ILogicalElement, ICollectionItemElement>)base.LogicalChildren)!.DerivedList;

        public ICollectionEntry BaseValueEntry => (ICollectionEntry)base.BaseValueEntry;
        public ICollectionEntry ValueEntry => (ICollectionEntry)base.ValueEntry;

        protected override bool CanHaveChildren()
        {
            return true;
        }

        protected override IReadOnlyElementList<ILogicalElement> CreateLogicalChildren()
        {
            var baseLogicalChildren = base.CreateLogicalChildren();
            _mutableLogicalChildren = (ElementList<ILogicalElement>)baseLogicalChildren;
            var wrapper = new ReadOnlyElementListWrapper<ICollectionItemElement, ILogicalElement>(baseLogicalChildren);
            return new ReadOnlyElementListBoxedWrapper<ILogicalElement, ICollectionItemElement>(wrapper);
        }

        protected override IValueEntry CreateBaseValueEntry()
        {
            var valueEntryType = (Definition.IsOrdered ? typeof(OrderedCollectionEntry<,>) : typeof(CollectionEntry<,>))
                .MakeGenericType(Definition.ValueType, Definition.ItemType);
            return valueEntryType.CreateInstance<IValueEntry>(this);
        }

        protected override IValueEntry CreateWrapperValueEntry()
        {
            var valueEntryType = (Definition.IsOrdered ? typeof(OrderedCollectionEntryWrapper<,,,>) : typeof(CollectionEntryWrapper<,,,>))
                .MakeGenericType(
                    BaseValueEntry.RuntimeValueType,
                    BaseValueEntry.RuntimeItemType,
                    BaseValueEntry.ValueType,
                    BaseValueEntry.ItemType);
            return valueEntryType.CreateInstance<IValueEntry>(BaseValueEntry);
        }

        protected override void PostProcessBaseValueEntry(IValueEntry baseValueEntry)
        {
            base.PostProcessBaseValueEntry(baseValueEntry);
            var collectionEntry = (ICollectionEntry)baseValueEntry;
            collectionEntry.AfterCollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            // UIToolkit uses incremental updates, IMGUI uses full refresh
            if (SharedContext.Tree.BackendMode == InspectorBackendMode.UIToolkit)
            {
                OnCollectionChangedIncremental(e);
            }
            else
            {
                RequestRefresh();
            }
        }

        private void OnCollectionChangedIncremental(CollectionChangedEventArgs e)
        {
            if (StructureResolver is not ICollectionStructureResolver collectionStructureResolver)
            {
                RequestRefresh();
                return;
            }

            switch (e.ChangeType)
            {
                case CollectionChangeType.Add:
                    HandleAddItem(collectionStructureResolver);
                    break;

                case CollectionChangeType.Remove:
                    HandleRemoveItem(collectionStructureResolver);
                    break;

                case CollectionChangeType.Insert:
                    HandleAddItem(collectionStructureResolver);
                    break;

                case CollectionChangeType.RemoveAt:
                    HandleRemoveItem(collectionStructureResolver);
                    break;

                case CollectionChangeType.Clear:
                    HandleClearCollection(collectionStructureResolver);
                    break;
            }
        }

        private void HandleAddItem(ICollectionStructureResolver structureResolver)
        {
            structureResolver.IncrementItemCount();
            var definition = (ICollectionItemDefinition)structureResolver.GetChildrenDefinitions()[^1];
            var newElement = SharedContext.Tree.ElementFactory.CreateCollectionItemElement(definition, this);
            _mutableLogicalChildren.Add(newElement);
        }

        private void HandleRemoveItem(ICollectionStructureResolver structureResolver)
        {
            var element = _mutableLogicalChildren[^1];
            _mutableLogicalChildren.RemoveAt(_mutableLogicalChildren.Count - 1);
            element.Destroy();
            structureResolver.DecrementItemCount();
        }

        private void HandleClearCollection(ICollectionStructureResolver structureResolver)
        {
            var children = new List<ILogicalElement>(_mutableLogicalChildren);
            _mutableLogicalChildren.Clear();
            foreach (var child in children)
            {
                child.Destroy();
            }
            structureResolver.ClearItemCount();
        }

        protected override void Dispose()
        {
            base.Dispose();
            _mutableLogicalChildren = null;
        }
    }
}
