using System.Reflection;
using EasyToolkit.Inspector.Attributes;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element builders that handle method-related attributes.
    /// Provides a convenient way to access the associated method attribute and validate method element types.
    /// </summary>
    /// <typeparam name="TAttribute">The type of method attribute this builder handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualMethodBuilder<TAttribute> : VisualAttributeBuilder<TAttribute>
        where TAttribute : MethodAttribute
    {
        /// <summary>
        /// Gets the method element associated with this builder.
        /// </summary>
        public new IMethodElement Element => base.Element as IMethodElement;

        /// <summary>
        /// Gets the method information for the method element.
        /// </summary>
        public MethodInfo MethodInfo => Element.Definition.MethodInfo;

        /// <summary>
        /// Determines whether this builder can handle the specified element.
        /// Validates that the element is a method element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected override bool CanBuild(IElement element)
        {
            if (element is IMethodElement methodElement)
            {
                return CanBuildElement(methodElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this builder can handle the specified method element.
        /// Override this method to provide custom validation logic.
        /// </summary>
        /// <param name="element">The method element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected virtual bool CanBuildElement(IMethodElement element)
        {
            return true;
        }
    }
}
