using System;
using System.Collections.Generic;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Collection definition implementation for describing collection data structures.
    /// Inherits from <see cref="ValueDefinition"/> and extends it with collection-specific metadata.
    /// </summary>
    public class CollectionDefinition : ValueDefinition, ICollectionDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDefinition"/> class.
        /// </summary>
        /// <param name="valueType">The type of the collection value.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="isOrdered">Whether this collection is ordered.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        public CollectionDefinition(
            Type valueType,
            Type itemType,
            bool isOrdered,
            string name,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : this(valueType, itemType, isOrdered, ElementRoles.Collection, name, additionalAttributes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionDefinition"/> class with extra roles.
        /// </summary>
        /// <param name="valueType">The type of the collection value.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="isOrdered">Whether this collection is ordered.</param>
        /// <param name="roles">The roles that describe the element.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="itemType"/> is null.</exception>
        protected CollectionDefinition(
            Type valueType,
            Type itemType,
            bool isOrdered,
            ElementRoles roles,
            string name,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(valueType, roles.Add(ElementRoles.Collection), name, additionalAttributes)
        {
            ItemType = itemType ?? throw new ArgumentNullException(nameof(itemType));
            IsOrdered = isOrdered;
        }

        /// <summary>
        /// Gets the type of elements contained in this collection.
        /// For dictionaries, this represents the type of values.
        /// </summary>
        public Type ItemType { get; }

        /// <summary>
        /// Gets a value indicating whether this collection is ordered (can be accessed by index).
        /// Ordered collections include arrays, lists, and other indexable sequences.
        /// Unordered collections include sets, dictionaries, and other non-indexable collections.
        /// </summary>
        public bool IsOrdered { get; }
    }
}
