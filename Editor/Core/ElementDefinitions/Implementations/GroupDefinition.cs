using System;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Group definition implementation for grouping abstract concepts in the inspector.
    /// Defines the start and end attributes that create logical groupings of elements.
    /// </summary>
    public sealed class GroupDefinition : ElementDefinition, IGroupDefinition
    {
        /// <summary>
        /// Gets or sets the type of the attribute that begins this group (e.g., <see cref="Attributes.GroupAttribute"/>).
        /// </summary>
        public Type GroupAttributeType { get; set; }
    }
}
