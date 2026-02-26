using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Vector4 value types.
    /// Creates a Vector4Field for editing 4D vector values in the inspector.
    /// </summary>
    public class VisualVector4Builder : VisualValueBuilder<Vector4>
    {
        /// <summary>
        /// Creates a Vector4Field configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new Vector4Field(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
