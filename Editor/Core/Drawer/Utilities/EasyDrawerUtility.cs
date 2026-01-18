using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Reflection;
using EasyToolKit.OdinSerializer;

namespace EasyToolKit.Inspector.Editor
{
    public static class EasyDrawerUtility
    {
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
            var key1 = TwoWaySerializationBinder.Default.BindToName(drawer.GetType());
            var key2 = ElementUtility.GetKey(drawer.Element);
            return string.Join("+", key1, key2);
        }
    }
}
