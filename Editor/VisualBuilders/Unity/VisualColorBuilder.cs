using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Color value types.
    /// Creates a ColorField for editing color values in the inspector.
    /// </summary>
    public class VisualColorBuilder : VisualValueBuilder<Color>
    {
        /// <summary>
        /// Creates a ColorField configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new ColorField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
