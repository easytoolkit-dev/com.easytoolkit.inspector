using System.Reflection;
using EasyToolkit.Inspector.Attributes;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element processors that handle method-related attributes.
    /// Provides a convenient way to access the associated method attribute and validate method element types.
    /// </summary>
    /// <typeparam name="TAttribute">The type of method attribute this processor handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualMethodProcessor<TAttribute> : VisualAttributeProcessor<TAttribute>
        where TAttribute : MethodAttribute
    {
        /// <summary>
        /// Gets the method element associated with this processor.
        /// </summary>
        public new IMethodElement Element => base.Element as IMethodElement;

        /// <summary>
        /// Gets the method information for the method element.
        /// </summary>
        public MethodInfo MethodInfo => Element.Definition.MethodInfo;

        /// <summary>
        /// Determines whether this processor can handle the specified element.
        /// Validates that the element is a method element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected override bool CanProcess(IElement element)
        {
            if (element is IMethodElement methodElement)
            {
                return CanProcessElement(methodElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this processor can handle the specified method element.
        /// Override this method to provide custom validation logic.
        /// </summary>
        /// <param name="element">The method element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected virtual bool CanProcessElement(IMethodElement element)
        {
            return true;
        }
    }
}
