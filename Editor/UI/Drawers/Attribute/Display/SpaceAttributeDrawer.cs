using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class SpaceAttributeDrawer : EasyAttributeDrawer<SpaceAttribute>
    {
        protected override void Draw(GUIContent label)
        {
            if (Attribute.height == 0)
            {
                EditorGUILayout.Space();
            }
            else
            {
                GUILayout.Space(Attribute.height);
            }

            CallNextDrawer(label);
        }
    }
}
