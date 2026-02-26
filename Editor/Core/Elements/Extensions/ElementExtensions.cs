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

        public static void Draw(this IElement element)
        {
            element.Draw(element.Label);
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

        /// <summary>
        /// Gets the owning VisualElement for this element by recursively traversing up the parent chain.
        /// Returns null if the backend mode is not UI Toolkit.
        /// </summary>
        /// <param name="element">The element to get the owning VisualElement for.</param>
        /// <returns>The owning VisualElement, or null if not in UI Toolkit mode or no parent has a VisualElement.</returns>
        [CanBeNull]
        public static VisualElement GetOwningVisualElement(this IElement element)
        {
            if (element.SharedContext.Tree.BackendMode != InspectorBackendMode.UIToolkit)
            {
                return null;
            }

            if (element is IRootElement)
            {
                return element.SharedContext.Tree.RootVisualElement;
            }

            var current = element;
            do
            {
                current = current.Parent;
                if (current == null)
                {
                    return null;
                }

                if (current.VisualElement != null)
                {
                    return current.VisualElement;
                }

                if (current is IRootElement)
                {
                    return element.SharedContext.Tree.RootVisualElement;
                }

            } while (false);

            return null;
        }
    }
}
