using System;
using System.Collections.Generic;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Root definition implementation for the inspector tree.
    /// An abstract concept similar to dynamically created values, used to represent Unity instances.
    /// </summary>
    public sealed class RootDefinition : ValueDefinition, IRootDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootDefinition"/> class.
        /// </summary>
        /// <param name="valueType">The type of the inspected root value.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        public RootDefinition(
            Type valueType,
            string name = "$Root$",
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(valueType, ElementRoles.Root, name, additionalAttributes)
        {
        }
    }
}
