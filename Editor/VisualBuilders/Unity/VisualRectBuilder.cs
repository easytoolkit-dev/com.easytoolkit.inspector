using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Rect value types.
    /// Creates a RectField for editing rectangle values in the inspector.
    /// </summary>
    public class VisualRectBuilder : VisualValueBuilder<Rect>
    {
        /// <summary>
        /// Creates a RectField configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new RectField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
