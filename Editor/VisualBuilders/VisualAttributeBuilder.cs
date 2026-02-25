using System;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Abstract base class for visual element builders that handle specific attribute types.
    /// Provides a convenient way to access the associated attribute and validate element types.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute this builder handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualAttributeBuilder<TAttribute> : VisualBuilder
        where TAttribute : Attribute
    {
        private TAttribute _attribute;

        /// <summary>
        /// Gets the logical element associated with this builder.
        /// </summary>
        public new ILogicalElement Element => (ILogicalElement)base.Element;

        /// <summary>
        /// Gets the attribute associated with this builder.
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
        /// Validates that the element is a logical element.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected override bool CanBuild(IElement element)
        {
            if (element is ILogicalElement logicalElement)
            {
                return CanBuildElement(logicalElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this builder can handle the specified logical element.
        /// Override this method to provide custom validation logic.
        /// </summary>
        /// <param name="element">The logical element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected virtual bool CanBuildElement(ILogicalElement element)
        {
            return true;
        }
    }

    /// <summary>
    /// Abstract base class for visual element builders that handle specific attribute types with value type constraints.
    /// Provides a convenient way to access the associated attribute and validate value types.
    /// </summary>
    /// <typeparam name="TAttribute">The type of attribute this builder handles.</typeparam>
    /// <typeparam name="TValue">The type of value this builder handles.</typeparam>
    [HandlerConstraints]
    public abstract class VisualAttributeBuilder<TAttribute, TValue> : VisualAttributeBuilder<TAttribute>
        where TAttribute : Attribute
    {
        private IValueEntry<TValue> _valueEntry;

        /// <summary>
        /// Gets the value element associated with this builder.
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
        /// Determines whether this builder can handle the specified element.
        /// Validates that the element is a value element with the correct value type.
        /// </summary>
        /// <param name="element">The element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected sealed override bool CanBuild(IElement element)
        {
            if (element is IValueElement valueElement)
            {
                var valueType = valueElement.ValueEntry.ValueType;
                return valueType == typeof(TValue) &&
                       CanBuildValueType(valueType) &&
                       CanBuildElement(valueElement);
            }

            return false;
        }

        /// <summary>
        /// Determines whether this builder can handle the specified value type.
        /// Override this method to provide custom value type validation logic.
        /// </summary>
        /// <param name="valueType">The value type to test.</param>
        /// <returns>True if this builder can handle the value type; otherwise, false.</returns>
        protected virtual bool CanBuildValueType(Type valueType)
        {
            return true;
        }

        /// <summary>
        /// Determines whether this builder can handle the specified value element.
        /// Override this method to provide custom element validation logic.
        /// </summary>
        /// <param name="element">The value element to test.</param>
        /// <returns>True if this builder can handle the element; otherwise, false.</returns>
        protected virtual bool CanBuildElement(IValueElement element)
        {
            return true;
        }
    }
}
