using System;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Enum value types.
    /// Creates an EnumField for editing enumeration values in the inspector.
    /// </summary>
    public class VisualEnumBuilder : VisualValueBuilder<Enum>
    {
        /// <summary>
        /// Creates an EnumField configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var enumType = ValueEntry.ValueType;
            var field = new EnumField(Element.Label.text, ValueEntry.SmartValue);
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }

        /// <summary>
        /// Determines whether this builder can handle the specified value type.
        /// Validates that the type is an enumeration.
        /// </summary>
        protected override bool CanBuildValueType(Type valueType)
        {
            return valueType.IsEnum;
        }
    }
}
