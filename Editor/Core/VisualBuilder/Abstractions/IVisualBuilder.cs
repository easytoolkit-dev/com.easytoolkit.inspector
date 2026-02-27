using System;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public interface IVisualBuilder : IHandler, IDisposable
    {
        VisualElement CreateVisualElement();
    }
}
