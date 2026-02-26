using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Vector3 value types.
    /// Creates a Vector3Field for editing 3D vector values in the inspector.
    /// </summary>
    public class VisualVector3Builder : VisualValueBuilder<Vector3>
    {
        /// <summary>
        /// Creates a Vector3Field configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new Vector3Field(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
