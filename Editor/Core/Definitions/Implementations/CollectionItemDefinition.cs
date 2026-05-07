using System;
using System.Collections.Generic;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Collection item definition implementation handling individual elements in collections.
    /// Similar to dynamically created custom values, representing individual element items in collections.
    /// </summary>
    public sealed class CollectionItemDefinition : ValueDefinition, ICollectionItemDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItemDefinition"/> class.
        /// </summary>
        /// <param name="valueType">The type of the collection item value.</param>
        /// <param name="itemIndex">The index of this item within its parent collection.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        public CollectionItemDefinition(
            Type valueType,
            int itemIndex,
            string name,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(valueType, ElementRoles.CollectionItem, name, additionalAttributes)
        {
            ItemIndex = itemIndex;
        }

        /// <summary>
        /// Gets the index of this item within its parent collection.
        /// </summary>
        public int ItemIndex { get; }
    }
}
