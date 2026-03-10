using System;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Configuration interface for creating group element definitions.
    /// Groups define logical sections in the inspector interface.
    /// </summary>
    public class GroupConfiguration : ElementConfiguration, IGroupConfiguration
    {
        /// <summary>
        /// Gets or sets the type of the attribute that begins this group
        /// (e.g., <see cref="Attributes.GroupAttribute"/>).
        /// </summary>
        public Type GroupAttributeType { get; set; }

        protected void ProcessDefinition(GroupDefinition definition)
        {
            if (GroupAttributeType == null)
            {
                throw new InvalidOperationException("GroupAttributeType cannot be null");
            }

            definition.Roles = definition.Roles.Add(ElementRoles.Group);
            definition.GroupAttributeType = GroupAttributeType;
            base.ProcessDefinition(definition);
        }

        /// <summary>
        /// Creates a new <see cref="IGroupDefinition"/> instance based on the current configuration.
        /// </summary>
        /// <returns>A new group definition instance.</returns>
        public IGroupDefinition CreateDefinition()
        {
            var definition = new GroupDefinition();
            ProcessDefinition(definition);
            return definition;
        }
    }
}
