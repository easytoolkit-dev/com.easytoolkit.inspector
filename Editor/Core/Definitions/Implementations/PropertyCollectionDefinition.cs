using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Property collection definition implementation that unifies <see cref="ICollectionDefinition"/> and <see cref="IPropertyDefinition"/>.
    /// Represents collection properties on an object, providing both collection-specific metadata and reflection information.
    /// </summary>
    public sealed class PropertyCollectionDefinition : CollectionDefinition, IPropertyCollectionDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyCollectionDefinition"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="System.Reflection.PropertyInfo"/> that represents this property.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="isOrdered">Whether this collection is ordered.</param>
        /// <param name="name">The element name. Uses the property name when null.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyInfo"/> is null.</exception>
        public PropertyCollectionDefinition(
            PropertyInfo propertyInfo,
            Type itemType,
            bool isOrdered,
            string name = null,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(
                (propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo))).PropertyType,
                itemType,
                isOrdered,
                ElementRoles.Property,
                name ?? propertyInfo.Name,
                additionalAttributes)
        {
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.PropertyInfo"/> that represents this property.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => PropertyInfo;
    }
}
