using System;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Group definition interface for grouping abstract concepts in the inspector.
    /// Defines the start and end attributes that create logical groupings of elements.
    /// </summary>
    public interface IGroupDefinition : IElementDefinition
    {
        /// <summary>
        /// Gets the type of the attribute that begins this group (e.g., <see cref="Attributes.GroupAttribute"/>).
        /// </summary>
        Type GroupAttributeType { get; }
    }
}
