using System;
using EasyToolkit.Core.Mathematics;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Attribute used to specify the priority of a visual processor.
    /// This attribute can be applied to visual processor classes to control their execution order
    /// in the visual processor chain. Higher priority visual processors are executed before lower priority visual processors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VisualProcessorPriorityAttribute : Attribute, IPriorityAccessor
    {
        /// <summary>
        /// Represents the lowest possible visual processor priority.
        /// </summary>
        public static readonly OrderPriority LowestPriority = new OrderPriority(VisualProcessorPriorityLevel.Lowest);

        /// <summary>
        /// Represents the standard priority for style processors.
        /// </summary>
        public static readonly OrderPriority StylePriority = new OrderPriority(VisualProcessorPriorityLevel.Style);

        /// <summary>
        /// Represents the priority for layout processors.
        /// </summary>
        public static readonly OrderPriority LayoutPriority = new OrderPriority(VisualProcessorPriorityLevel.Layout);

        /// <summary>
        /// Represents the highest standard visual processor priority.
        /// </summary>
        public static readonly OrderPriority SuperPriority = new OrderPriority(VisualProcessorPriorityLevel.Super);

        /// <summary>
        /// Gets the priority value for the visual processor.
        /// </summary>
        public OrderPriority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualProcessorPriorityAttribute"/> class.
        /// </summary>
        /// <param name="value">The priority value for the visual processor. Defaults to <see cref="VisualProcessorPriorityLevel.Lowest"/>.</param>
        public VisualProcessorPriorityAttribute(double value = VisualProcessorPriorityLevel.Lowest)
        {
            Priority = new OrderPriority(value);
        }
    }
}
