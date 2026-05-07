using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Property definition implementation that handles <see cref="System.Reflection.PropertyInfo"/>.
    /// Provides consistent access to property reflection information.
    /// </summary>
    public sealed class PropertyDefinition : ValueDefinition, IPropertyDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDefinition"/> class.
        /// </summary>
        /// <param name="propertyInfo">The <see cref="System.Reflection.PropertyInfo"/> that represents this property.</param>
        /// <param name="name">The element name. Uses the property name when null.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyInfo"/> is null.</exception>
        public PropertyDefinition(
            PropertyInfo propertyInfo,
            string name = null,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(
                (propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo))).PropertyType,
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
