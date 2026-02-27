using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolkit.Core;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public static class ElementExtensions
    {
        public static IValueElement CastValue(this IElement element)
        {
            return Cast<IValueElement>(element);
        }

        private static T Cast<T>(this IElement element)
            where T : IElement
        {
            if (element is T e)
            {
                return e;
            }
            throw new InvalidCastException($"Element '{element}' is not of type '{typeof(T)}'");
        }

        public static void Draw(this IElement element, bool forceDraw = false)
        {
            element.Draw(element.Label, forceDraw);
        }

        public static LocalPersistentContext<T> GetPersistentContext<T>(this IElement element, string key, T defaultValue = default)
        {
            var key1 = ElementUtility.GetKey(element);
            return PersistentContext.GetLocal(string.Join("+", key1, key), defaultValue);
        }

        public static ElementAttributeInfo GetAttributeInfo(this IElement element, Type attributeType)
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                if (attributeInfo.Attribute.GetType() == attributeType)
                {
                    return attributeInfo;
                }
            }

            return null;
        }
        public static Attribute GetAttribute(this IElement element, Type attributeType, bool includeDerived = false)
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                if (includeDerived)
                {
                    if (attributeInfo.Attribute.GetType().IsDerivedFrom(attributeType))
                    {
                        return attributeInfo.Attribute;
                    }
                }
                else
                {
                    if (attributeInfo.Attribute.GetType() == attributeType)
                    {
                        return attributeInfo.Attribute;
                    }
                }
            }

            return null;
        }

        public static TAttribute GetAttribute<TAttribute>(this IElement element, bool includeDerived = false) where TAttribute : Attribute
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                if (attributeInfo.Attribute is TAttribute attribute)
                {
                    if (!includeDerived && attributeInfo.Attribute.GetType() != typeof(TAttribute))
                    {
                        continue;
                    }
                    return attribute;
                }
            }

            return null;
        }

        public static IEnumerable<Attribute> EnumerateAttributes(this IElement element)
        {
            foreach (var attributeInfo in element.GetAttributeInfos())
            {
                yield return attributeInfo.Attribute;
            }
        }
    }
}
