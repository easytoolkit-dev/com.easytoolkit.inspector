using System;
using EasyToolKit.Core.Mathematics;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Attribute used to specify the priority of a post processor.
    /// This attribute can be applied to post processor classes to control their execution order
    /// in the post processor chain. Higher priority post processors are executed before lower priority post processors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PostProcessorPriorityAttribute : Attribute, IPriorityAccessor
    {
        /// <summary>
        /// Represents the lowest possible post processor priority.
        /// </summary>
        public static readonly OrderPriority LowestPriority = new OrderPriority(PostProcessorPriorityLevel.Lowest);

        /// <summary>
        /// Represents the standard priority for validation post processors.
        /// </summary>
        public static readonly OrderPriority ValidationPriority = new OrderPriority(PostProcessorPriorityLevel.Validation);

        /// <summary>
        /// Represents the priority for cleanup post processors.
        /// </summary>
        public static readonly OrderPriority CleanupPriority = new OrderPriority(PostProcessorPriorityLevel.Cleanup);

        /// <summary>
        /// Represents the highest standard post processor priority.
        /// </summary>
        public static readonly OrderPriority SuperPriority = new OrderPriority(PostProcessorPriorityLevel.Super);

        /// <summary>
        /// Gets the priority value for the post processor.
        /// </summary>
        public OrderPriority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostProcessorPriorityAttribute"/> class.
        /// </summary>
        /// <param name="value">The priority value for the post processor..</param>
        public PostProcessorPriorityAttribute(double value = 0.0)
        {
            Priority = new OrderPriority(value);
        }
    }
}
