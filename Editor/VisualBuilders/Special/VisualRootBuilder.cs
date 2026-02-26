using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public class VisualRootBuilder<T> : VisualValueBuilder<T>
        where T : UnityEngine.Object
    {
        protected override bool CanBuildElement(IValueElement element)
        {
            return element is IRootElement;
        }

        public override VisualElement CreateVisualElement()
        {
            return null;
        }
    }
}
