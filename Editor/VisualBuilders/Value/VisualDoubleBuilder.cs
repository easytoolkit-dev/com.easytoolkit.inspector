using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for double value types.
    /// Creates a DoubleField for editing double precision floating point values in the inspector.
    /// </summary>
    public class VisualDoubleBuilder : VisualValueBuilder<double>
    {
        /// <summary>
        /// Creates a DoubleField configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new DoubleField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
