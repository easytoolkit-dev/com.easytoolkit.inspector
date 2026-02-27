using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolkit.Inspector.Editor
{
    [ResolverPriority(1.0)]
    [HandlerConstraints]
    public abstract class CollectionStructureResolverBase<TCollection> : ValueStructureResolverBase<TCollection>, ICollectionStructureResolver
    {
        private ICollectionItemDefinition[] _definitions;

        /// <summary>
        /// Gets the type of elements in the collection
        /// </summary>
        public abstract Type ItemType { get; }

        protected override bool CanResolveElement(IValueElement element)
        {
            return element.Definition.Roles.IsCollection();
        }

        protected override void Initialize()
        {
            var count = CalculateChildCount();
            _definitions = new ICollectionItemDefinition[count];
            for (int i = 0; i < count; i++)
            {
                _definitions[i] = CreateItemDefinition(i);
            }
        }

        public void IncrementItemCount()
        {
            UpdateItemCount(_definitions.Length + 1);
        }

        public void DecrementItemCount()
        {
            UpdateItemCount(_definitions.Length - 1);
        }

        public void ClearItemCount()
        {
            UpdateItemCount(0);
        }

        protected override IElementDefinition[] GetChildrenDefinitions()
        {
            return _definitions;
        }

        protected virtual ICollectionItemDefinition CreateItemDefinition(int itemIndex)
        {
            return InspectorElements.Configurator.CollectionItem()
                .WithItemIndex(itemIndex)
                .WithValueType(ItemType)
                .WithName($"Array.data[{itemIndex}]")
                .CreateDefinition();
        }

        /// <summary>
        /// Clears the cached collection item definitions when the resolver is returned to the pool.
        /// </summary>
        protected override void OnRelease()
        {
            base.OnRelease();
            _definitions = null;
        }

        protected abstract int CalculateChildCount();

        private void UpdateItemCount(int count)
        {
            if (count != _definitions.Length)
            {
                var originalCount = _definitions.Length;
                Array.Resize(ref _definitions, count);
                if (count > originalCount)
                {
                    for (int i = originalCount; i < count; i++)
                    {
                        _definitions[i] = CreateItemDefinition(i);
                    }
                }
            }
        }
    }
}
