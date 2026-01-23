using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorDrawerUtility
    {
        [InitializeOnLoad]
        private class NullPriorityFallbackInitializer
        {
            static NullPriorityFallbackInitializer()
            {
                HandlerUtility.AddNullPriorityFallback(type =>
                {
                    if (type.IsImplementsGenericDefinition(typeof(EasyAttributeDrawer<>)))
                    {
                        return DrawerPriorityAttribute.AttributePriority;
                    }
                    if (type.IsImplementsGenericDefinition(typeof(EasyGroupAttributeDrawer<>)))
                    {
                        return DrawerPriorityAttribute.AttributePriority;
                    }
                    if (type.IsImplementsGenericDefinition(typeof(EasyValueDrawer<>)))
                    {
                        return DrawerPriorityAttribute.ValuePriority;
                    }

                    return null;
                });
            }
        }

        private static readonly Dictionary<Type, Type> UnityPropertyDrawerTypesByDrawnType =
            new Dictionary<Type, Type>();

        private static readonly FieldInfo CustomPropertyDrawerTypeFieldInfo =
            typeof(UnityEditor.CustomPropertyDrawer).GetField("m_Type", MemberAccessFlags.AllInstance);

        static InspectorDrawerUtility()
        {
            foreach (var type in AssemblyUtility.GetTypesExcept(AssemblyCategory.System)
                         .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && t.IsDerivedFrom<UnityEditor.PropertyDrawer>()))
            {
                var customPropertyDrawers = type.GetCustomAttributes<UnityEditor.CustomPropertyDrawer>();
                foreach (var drawer in customPropertyDrawers)
                {
                    var drawnType = (Type)CustomPropertyDrawerTypeFieldInfo.GetValue(drawer);
                    if (drawnType != null)
                    {
                        UnityPropertyDrawerTypesByDrawnType[drawnType] = type;
                    }
                }
            }
        }

        public static bool IsDefinedUnityPropertyDrawer(Type targetType)
        {
            if (UnityPropertyDrawerTypesByDrawnType.ContainsKey(targetType))
            {
                return true;
            }

            //TODO 通过CustomPropertyDrawer判断是否需要检查继承
            foreach (var drawnType in UnityPropertyDrawerTypesByDrawnType.Keys)
            {
                if (drawnType.IsAssignableFrom(targetType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
