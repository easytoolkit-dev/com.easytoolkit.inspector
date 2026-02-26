using System;
using System.Collections.Generic;
using EasyToolkit.Core;
using EasyToolkit.Core.Reflection;
using UnityEditor;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Utility class for visual processor operations.
    /// </summary>
    public static class VisualProcessorUtility
    {
        [InitializeOnLoad]
        private class NullPriorityFallbackInitializer
        {
            static NullPriorityFallbackInitializer()
            {
                HandlerUtility.AddNullPriorityFallback(type =>
                {
                    if (type.IsImplementsGenericDefinition(typeof(VisualAttributeProcessor<>)))
                    {
                        return VisualProcessorPriorityAttribute.AttributePriority;
                    }
                    if (type.IsImplementsGenericDefinition(typeof(VisualGroupProcessor<>)))
                    {
                        return VisualProcessorPriorityAttribute.AttributePriority;
                    }
                    if (type.IsImplementsGenericDefinition(typeof(VisualValueProcessor<>)))
                    {
                        return VisualProcessorPriorityAttribute.ValuePriority;
                    }

                    return null;
                });
            }
        }

        /// <summary>
        /// Gets the visual processor types that can handle the specified element.
        /// </summary>
        /// <param name="element">The element to get visual processor types for.</param>
        /// <returns>A collection of visual processor types.</returns>
        public static IEnumerable<Type> GetVisualProcessorTypes(IElement element)
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

            return HandlerUtility.GetHandlerTypes(element, type => type.IsDerivedFrom<IVisualProcessor>(), additionalMatchTypesList);
        }

        /// <summary>
        /// Gets the unique key for a visual processor based on its type and associated element.
        /// </summary>
        /// <param name="processor">The visual processor to generate a key for.</param>
        /// <returns>A unique string key combining the processor type and element key.</returns>
        public static string GetKey(IVisualProcessor processor)
        {
            var key1 = TypeUtility.GetTypeName(processor.GetType());
            var key2 = ElementUtility.GetKey(processor.Element);
            return string.Join("+", key1, key2);
        }
    }
}
