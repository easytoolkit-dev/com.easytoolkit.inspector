using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for Bounds value types.
    /// Creates a BoundsField for editing axis-aligned bounding box values in the inspector.
    /// </summary>
    public class VisualBoundsBuilder : VisualValueBuilder<Bounds>
    {
        /// <summary>
        /// Creates a BoundsField configured for the current value entry.
        /// </summary>
        public override VisualElement CreateVisualElement()
        {
            var field = new BoundsField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
