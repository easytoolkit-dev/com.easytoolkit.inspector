using System;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Extension methods for <see cref="IGroupConfiguration"/> interfaces.
    /// Provides fluent API methods for configuring group element properties.
    /// </summary>
    public static class GroupConfigurationExtensions
    {
        /// <summary>
        /// Sets the group attribute type for a group configuration.
        /// All groups use the unified <see cref="EndGroupAttribute"/> for ending.
        /// </summary>
        /// <typeparam name="TConfiguration">The group configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <param name="groupAttributeType">The attribute type that begins the group.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithGroupAttribute<TConfiguration>(this TConfiguration configuration, Type groupAttributeType)
            where TConfiguration : IGroupConfiguration
        {
            configuration.GroupAttributeType = groupAttributeType;
            return configuration;
        }

        /// <summary>
        /// Sets the group attribute type for a group configuration using generic type parameter.
        /// All groups use the unified <see cref="EndGroupAttribute"/> for ending.
        /// </summary>
        /// <typeparam name="TGroupAttribute">The attribute type that begins the group.</typeparam>
        /// <typeparam name="TConfiguration">The group configuration type.</typeparam>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The configuration instance for method chaining.</returns>
        public static TConfiguration WithGroupAttribute<TGroupAttribute, TConfiguration>(this TConfiguration configuration)
            where TGroupAttribute : Attribute
            where TConfiguration : IGroupConfiguration
        {
            configuration.GroupAttributeType = typeof(TGroupAttribute);
            return configuration;
        }
    }
}
