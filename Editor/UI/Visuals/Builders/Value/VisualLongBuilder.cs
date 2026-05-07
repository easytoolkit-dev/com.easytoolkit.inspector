using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for long value types.
    /// Creates a LongField for editing long integer values in the inspector.
    /// </summary>
    public class VisualLongBuilder : VisualValueBuilder<long>
    {
        /// <summary>
        /// Creates a LongField configured for the current value entry.
        /// </summary>
        protected override VisualElement CreateVisualElement()
        {
            var field = new LongField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
