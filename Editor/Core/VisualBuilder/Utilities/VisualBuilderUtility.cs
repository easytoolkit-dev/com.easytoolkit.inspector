using System;
using System.Collections.Generic;
using EasyToolkit.Core.Reflection;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Utility class for visual builder operations.
    /// </summary>
    public static class VisualBuilderUtility
    {
        /// <summary>
        /// Gets the first visual builder type that can handle the specified element.
        /// </summary>
        /// <param name="element">The element to get visual builder type for.</param>
        /// <returns>The first matching visual builder type, or null if none found.</returns>
        public static Type GetVisualBuilderType(IElement element)
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

            return HandlerUtility.GetFirstHandlerType(element, type => type.IsDerivedFrom<IVisualBuilder>(), additionalMatchTypesList);
        }
    }
}
