using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    [VisualBuilderPriority(VisualBuilderPriorityLevel.Attribute - 1)]
    public class VisualRootBuilder<T> : VisualValueBuilder<T>
        where T : UnityEngine.Object
    {
        protected override bool CanBuildElement(IValueElement element)
        {
            return element is IRootElement;
        }

        protected override VisualElement CreateVisualElement()
        {
            foreach (var child in Element.Children)
            {
                child.Draw(child.Label);
            }
            return null;
        }
    }
}
