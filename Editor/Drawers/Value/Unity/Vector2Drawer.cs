using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    public class Vector2Drawer : EasyValueDrawer<Vector2>
    {
        protected override void Draw(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Vector2Field(label, value);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
