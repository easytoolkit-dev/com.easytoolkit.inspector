using EasyToolkit.Inspector.Attributes;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element builders that handle group-related attributes.
    /// Provides a convenient way to access the associated group attribute and validate group element types.
    /// </summary>
    /// <typeparam name="TAttribute">The type of begin group attribute this builder handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualGroupBuilder<TAttribute> : VisualBuilder
        where TAttribute : BeginGroupAttribute
    {
        private TAttribute _attribute;

        /// <summary>
        /// Gets the group element associated with this builder.
        /// </summary>
        public new IGroupElement Element => (IGroupElement)base.Element;

        /// <summary>
        /// Gets the begin group attribute associated with this builder.
        /// </summary>
        public TAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = Element.GetAttribute<TAttribute>();
                }

                return _attribute;
            }
        }

        /// <summary>
        /// Determines whether this builder can handle the specified element.
        /// Validates that the element is a group element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected override bool CanBuild(IElement element)
        {
            if (element is IGroupElement groupElement)
            {
                return CanBuildElement(groupElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this builder can handle the specified group element.
        /// Override this method to provide custom validation logic.
        /// </summary>
        /// <param name="element">The group element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected virtual bool CanBuildElement(IGroupElement element)
        {
            return true;
        }
    }
}
