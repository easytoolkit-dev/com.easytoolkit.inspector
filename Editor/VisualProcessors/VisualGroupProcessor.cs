using EasyToolkit.Inspector.Attributes;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element processors that handle group-related attributes.
    /// Provides a convenient way to access the associated group attribute and validate group element types.
    /// </summary>
    /// <typeparam name="TAttribute">The type of begin group attribute this processor handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualGroupProcessor<TAttribute> : VisualProcessor
        where TAttribute : BeginGroupAttribute
    {
        private TAttribute _attribute;

        /// <summary>
        /// Gets the group element associated with this processor.
        /// </summary>
        public new IGroupElement Element => (IGroupElement)base.Element;

        /// <summary>
        /// Gets the begin group attribute associated with this processor.
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
        /// Determines whether this processor can handle the specified element.
        /// Validates that the element is a group element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected override bool CanProcess(IElement element)
        {
            if (element is IGroupElement groupElement)
            {
                return CanProcessElement(groupElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this processor can handle the specified group element.
        /// Override this method to provide custom validation logic.
        /// </summary>
        /// <param name="element">The group element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected virtual bool CanProcessElement(IGroupElement element)
        {
            return true;
        }
    }
}
