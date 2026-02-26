using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for string value types.
    /// Creates a TextField for editing string values in the inspector.
    /// </summary>
    public class VisualStringBuilder : VisualValueBuilder<string>
    {
        /// <summary>
        /// Creates a TextField configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new TextField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
