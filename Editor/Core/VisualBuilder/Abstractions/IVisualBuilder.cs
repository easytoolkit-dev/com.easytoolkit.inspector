using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public interface IVisualBuilder : IHandler
    {
        VisualElement CreateVisualElement();
    }
}
