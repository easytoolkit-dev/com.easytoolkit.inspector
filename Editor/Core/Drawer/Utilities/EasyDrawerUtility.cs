using System;
using System.Collections.Generic;
using EasyToolkit.Core;
using EasyToolkit.Core.Reflection;
using UnityEditor;

namespace EasyToolkit.Inspector.Editor
{
    public static class EasyDrawerUtility
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
                    if (type.IsImplementsGenericDefinition(typeof(EasyGroupDrawer<>)))
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

        public static IEnumerable<Type> GetDrawerTypes(IElement element)
        {
            var additionalMatchTypesList = new List<Type[]>();
            foreach (var attribute in element.EnumerateAttributes())
            {
                additionalMatchTypesList.Add(new[] { attribute.GetType() });

                if (element is IValueElement valueElement)
                {
                    additionalMatchTypesList.Add(new[] { attribute.GetType(), valueElement.ValueEntry.ValueType });
                }
            }

            return HandlerUtility.GetHandlerTypes(element, type => type.IsDerivedFrom<IEasyDrawer>(), additionalMatchTypesList);
        }

        public static string GetKey(IEasyDrawer drawer)
        {
            var key1 = TypeUtility.GetTypeName(drawer.GetType());
            var key2 = ElementUtility.GetKey(drawer.Element);
            return string.Join("+", key1, key2);
        }
    }
}
