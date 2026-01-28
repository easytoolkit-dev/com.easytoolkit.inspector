using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    public interface IEasyDrawer : IHandler
    {
        DrawerChain Chain { get; set; }

        bool SkipWhenDrawing { get; set; }

        void Draw(GUIContent label);
    }
}
