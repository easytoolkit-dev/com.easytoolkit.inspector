using System;
using EasyToolkit.Core.Mathematics;

namespace EasyToolkit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResolverPriorityAttribute : Attribute, IPriorityAccessor
    {
        public OrderPriority Priority { get; }

        public ResolverPriorityAttribute(double priority = 0.0)
        {
            Priority = new OrderPriority(priority);
        }
    }
}
