using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for float value types.
    /// Creates a FloatField for editing float values in the inspector.
    /// </summary>
    public class VisualFloatBuilder : VisualValueBuilder<float>
    {
        /// <summary>
        /// Creates a FloatField configured for the current value entry.
        /// </summary>
        protected override VisualElement CreateVisualElement()
        {
            var field = new FloatField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
