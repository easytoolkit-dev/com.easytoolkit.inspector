using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Represents the definition of an element in the inspector hierarchy.
    /// It serves as the base interface for all element definitions.
    /// </summary>
    public abstract class ElementDefinition : IElementDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDefinition"/> class.
        /// </summary>
        /// <param name="roles">The roles that describe the element.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
        protected ElementDefinition(ElementRoles roles, string name, IReadOnlyList<Attribute> additionalAttributes = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
            }

            Roles = roles;
            Name = name;
            AdditionalAttributes = additionalAttributes?.ToArray();
        }

        /// <summary>
        /// Gets the flags of the element.
        /// </summary>
        public ElementRoles Roles { get; }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the additional attributes for customizing the element behavior.
        /// </summary>
        public IReadOnlyList<Attribute> AdditionalAttributes { get; }
    }
}
