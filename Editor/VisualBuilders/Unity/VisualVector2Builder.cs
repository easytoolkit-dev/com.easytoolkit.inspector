using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Vector2 value types.
    /// Creates a Vector2Field for editing 2D vector values in the inspector.
    /// </summary>
    public class VisualVector2Builder : VisualValueBuilder<Vector2>
    {
        /// <summary>
        /// Creates a Vector2Field configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new Vector2Field(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
