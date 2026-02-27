using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public class VisualIntBuilder : VisualValueBuilder<int>
    {
        protected override VisualElement CreateVisualElement()
        {
            var field = new IntegerField(Element.Label.text);
            field.value = ValueEntry.SmartValue;
            field.RegisterValueChangedCallback(evt =>
            {
                ValueEntry.SmartValue = evt.newValue;
            });
            return field;
        }
    }
}
