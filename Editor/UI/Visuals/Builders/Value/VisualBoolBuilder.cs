using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for bool value types.
    /// Creates a Toggle for editing boolean values in the inspector.
    /// </summary>
    public class VisualBoolBuilder : VisualValueBuilder<bool>
    {
        /// <summary>
        /// Creates a Toggle configured for the current value entry.
        /// </summary>
        protected override VisualElement CreateVisualElement()
        {
            var field = new Toggle(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
