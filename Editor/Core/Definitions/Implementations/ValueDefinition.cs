using System;
using System.Collections.Generic;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base implementation of <see cref="IValueDefinition"/> for all data-containing elements.
    /// </summary>
    public class ValueDefinition : ElementDefinition, IValueDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefinition"/> class.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        public ValueDefinition(Type valueType, string name, IReadOnlyList<Attribute> additionalAttributes = null)
            : this(valueType, ElementRoles.Value, name, additionalAttributes)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueDefinition"/> class with extra roles.
        /// </summary>
        /// <param name="valueType">The type of the value.</param>
        /// <param name="roles">The roles that describe the element.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="valueType"/> is null.</exception>
        protected ValueDefinition(
            Type valueType,
            ElementRoles roles,
            string name,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(roles.Add(ElementRoles.Value), name, additionalAttributes)
        {
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        public Type ValueType { get; }
    }
}
