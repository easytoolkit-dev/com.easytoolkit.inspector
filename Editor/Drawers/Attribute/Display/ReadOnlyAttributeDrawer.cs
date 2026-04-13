using EasyToolkit.Inspector.Attributes;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10)]
    public class ReadOnlyAttributeDrawer : EasyAttributeDrawer<ReadOnlyAttribute>
    {
        protected override void Draw(GUIContent label)
        {
            GUI.enabled = false;
            CallNextDrawer(label);
            GUI.enabled = true;
        }
    }
}
