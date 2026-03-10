using System;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Configuration interface for creating group element definitions.
    /// Groups organize related inspector elements using begin/end attribute pairs.
    /// </summary>
    public interface IGroupConfiguration : IElementConfiguration
    {
        /// <summary>
        /// Gets or sets the type of attribute that begins this group (e.g., <see cref="Attributes.GroupAttribute"/>).
        /// This attribute marks the start of a logical grouping in the inspector.
        /// </summary>
        Type GroupAttributeType { get; set; }

        /// <summary>
        /// Creates a new <see cref="IGroupDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new group definition instance.</returns>
        IGroupDefinition CreateDefinition();
    }
}
