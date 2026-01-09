using System;
using EasyToolKit.Core.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class ConflictedValueDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueType(Type valueType)
        {
            return valueType.IsBasicValueType() || valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        protected override void Draw(GUIContent label)
        {
            if (ValueEntry.State == ValueEntryState.Mixed)
            {
                EditorGUI.showMixedValue = true;
            }

            CallNextDrawer(label);
            EditorGUI.showMixedValue = false;
        }
    }
}
