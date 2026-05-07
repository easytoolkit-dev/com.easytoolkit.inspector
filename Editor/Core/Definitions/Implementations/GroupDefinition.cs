using System;
using System.Collections.Generic;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Group definition implementation for grouping abstract concepts in the inspector.
    /// Defines the start and end attributes that create logical groupings of elements.
    /// </summary>
    public sealed class GroupDefinition : ElementDefinition, IGroupDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupDefinition"/> class.
        /// </summary>
        /// <param name="groupAttributeType">The type of the attribute that begins this group.</param>
        /// <param name="name">The element name.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="groupAttributeType"/> is null.</exception>
        public GroupDefinition(
            Type groupAttributeType,
            string name,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(ElementRoles.Group, name, additionalAttributes)
        {
            GroupAttributeType = groupAttributeType ?? throw new ArgumentNullException(nameof(groupAttributeType));
        }

        /// <summary>
        /// Gets the type of the attribute that begins this group (e.g., <see cref="Attributes.GroupAttribute"/>).
        /// </summary>
        public Type GroupAttributeType { get; }
    }
}
