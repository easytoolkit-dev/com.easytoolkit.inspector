namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Provides standard priority levels for visual processors.
    /// These constants define the standard priority ranges used by different types of visual processors.
    /// Higher values indicate higher priority (executed earlier in the visual processor chain).
    /// </summary>
    public static class VisualProcessorPriorityLevel
    {
        /// <summary>
        /// The lowest priority level for visual processors that should execute last.
        /// </summary>
        public const double Lowest = -100000.0;

        /// <summary>
        /// The standard priority level for value processors that handle visual styling.
        /// </summary>
        public const double Value = 100000.0;

        /// <summary>
        /// The priority level for attribute processors that handle element attribute and arrangement.
        /// </summary>
        public const double Attribute = 200000.0;

        /// <summary>
        /// The highest standard priority level for visual processors that should execute first.
        /// </summary>
        public const double Super = 300000.0;
    }
}
