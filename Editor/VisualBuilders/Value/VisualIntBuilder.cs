using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public class VisualIntBuilder : VisualValueBuilder<int>
    {
        public override VisualElement CreateVisualElement()
        {
            var field = new IntegerField(Element.Label.text);
            field.RegisterValueChangedCallback(evt =>
            {
                Element.ValueEntry.WeakSmartValue = evt.newValue;
            });
            return field;
        }
    }
}
