using System;
using EasyToolkit.Core.Mathematics;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Attribute used to specify the priority of a visual builder.
    /// This attribute can be applied to visual builder classes to control their execution order
    /// in the visual builder chain. Higher priority visual builders are executed before lower priority visual builders.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class VisualBuilderPriorityAttribute : Attribute, IPriorityAccessor
    {
        /// <summary>
        /// Represents the lowest possible visual builder priority.
        /// </summary>
        public static readonly OrderPriority LowestPriority = new OrderPriority(VisualBuilderPriorityLevel.Lowest);

        /// <summary>
        /// Represents the standard priority for value builders.
        /// </summary>
        public static readonly OrderPriority ValuePriority = new OrderPriority(VisualBuilderPriorityLevel.Value);

        /// <summary>
        /// Represents the priority for attribute builders.
        /// </summary>
        public static readonly OrderPriority AttributePriority = new OrderPriority(VisualBuilderPriorityLevel.Attribute);

        /// <summary>
        /// Represents the highest standard visual builder priority.
        /// </summary>
        public static readonly OrderPriority SuperPriority = new OrderPriority(VisualBuilderPriorityLevel.Super);

        /// <summary>
        /// Gets the priority value for the visual builder.
        /// </summary>
        public OrderPriority Priority { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBuilderPriorityAttribute"/> class.
        /// </summary>
        /// <param name="value">The priority value for the visual builder. Defaults to <see cref="VisualBuilderPriorityLevel.Lowest"/>.</param>
        public VisualBuilderPriorityAttribute(double value = VisualBuilderPriorityLevel.Lowest)
        {
            Priority = new OrderPriority(value);
        }
    }
}
