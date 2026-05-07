using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Visual element builder for UnityEngine.Object reference types.
    /// Creates an ObjectField for editing object references in the inspector.
    /// </summary>
    public class VisualUnityObjectBuilder<T> : VisualValueBuilder<T>
        where T : UnityEngine.Object
    {
        /// <summary>
        /// Creates an ObjectField configured for the current value entry.
        /// </summary>
        /// <returns>A configured ObjectField for editing the object reference.</returns>
        protected override VisualElement CreateVisualElement()
        {
            var objectType = ValueEntry.ValueType;
            var field = new ObjectField(Element.Label.text)
            {
                objectType = objectType,
                value = ValueEntry.SmartValue
            };
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = (T)evt.newValue;
            });
            return field;
        }
    }
}
