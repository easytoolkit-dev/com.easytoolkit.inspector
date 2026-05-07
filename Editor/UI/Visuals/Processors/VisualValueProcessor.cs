using System;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element processors that handle specific value types.
    /// Provides a convenient way to access the value entry and validate value types.
    /// </summary>
    /// <typeparam name="TValue">The type of value this processor handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualValueProcessor<TValue> : VisualProcessor
    {
        private IValueEntry<TValue> _valueEntry;

        /// <summary>
        /// Gets the value element associated with this processor.
        /// </summary>
        public new IValueElement Element => base.Element as IValueElement;

        /// <summary>
        /// Gets the value entry for accessing and modifying the value.
        /// </summary>
        public IValueEntry<TValue> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Element.ValueEntry as IValueEntry<TValue>;
                }

                return _valueEntry;
            }
        }

        /// <summary>
        /// Determines whether this processor can handle the specified element.
        /// Validates that the element is a value element with the correct value type.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected sealed override bool CanProcess(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                var valueType = valueElement.ValueEntry.ValueType;
                return valueType == typeof(TValue) &&
                       CanProcessValueType(valueType) &&
                       CanProcessElement(valueElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this processor can handle the specified value type.
        /// Override this method to provide custom value type validation logic.
        /// </summary>
        /// <param name="valueType">The value type to test.</param>
        /// <returns>True if this processor can handle the value type; otherwise, false.</returns>
        protected virtual bool CanProcessValueType(Type valueType)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this processor can handle the specified value element.
        /// Override this method to provide custom element validation logic.
        /// </summary>
        /// <param name="element">The value element to test.</param>
        /// <returns>True if this processor can handle the element; otherwise, false.</returns>
        protected virtual bool CanProcessElement(IValueElement element)
        {
            return true;
        }
    }
}
