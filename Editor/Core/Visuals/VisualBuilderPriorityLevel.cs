namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Provides standard priority levels for visual builders.
    /// These constants define the standard priority ranges used by different types of visual builders.
    /// Higher values indicate higher priority (executed earlier in the visual builder chain).
    /// </summary>
    public static class VisualBuilderPriorityLevel
    {
        /// <summary>
        /// The lowest priority level for visual builders that should execute last.
        /// </summary>
        public const double Lowest = -100000.0;

        /// <summary>
        /// The standard priority level for value builders that handle visual styling.
        /// </summary>
        public const double Value = 100000.0;

        /// <summary>
        /// The priority level for attribute builders that handle element attribute and arrangement.
        /// </summary>
        public const double Attribute = 200000.0;

        /// <summary>
        /// The highest standard priority level for visual builders that should execute first.
        /// </summary>
        public const double Super = 300000.0;
    }
}
