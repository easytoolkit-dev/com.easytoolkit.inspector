using UnityEngine;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    public interface IEasyDrawer : IHandler
    {
        DrawerChain Chain { get; set; }

        bool SkipWhenDrawing { get; set; }

        void Draw(GUIContent label);
    }
}
