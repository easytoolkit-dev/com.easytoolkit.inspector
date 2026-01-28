using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    public class HideLabelAttributeDrawer : EasyAttributeDrawer<HideLabelAttribute>
    {
        protected override void Draw(GUIContent label)
        {
            CallNextDrawer(null);
        }
    }
}
