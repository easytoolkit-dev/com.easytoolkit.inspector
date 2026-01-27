using EasyToolKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core.Mathematics;
using EasyToolKit.Core.Reflection;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    /// <summary>
    /// Utility class for discovering and matching inspector elements that implement <see cref="IHandler"/>.
    /// Elements are discovered via reflection, sorted by priority obtained from <see cref="IPriorityAccessor"/> attributes,
    /// and registered in a <see cref="s_typeMatcher"/> for type-based matching.
    /// </summary>
    public static class HandlerUtility
    {
        private static Type[] s_elementTypes;
        private static ITypeMatcher s_typeMatcher;
        private static bool s_typeMatcherInitialized;
        private static readonly object InitializationLock = new object();

        /// <summary>
        /// Gets or sets a callback that provides a default <see cref="OrderPriority"/> when no priority attribute is found.
        /// If not set, returns null when no priority attribute is present.
        /// </summary>
        private static readonly List<Func<Type, OrderPriority?>> NullPriorityFallbacks = new List<Func<Type, OrderPriority?>>();

        public static ITypeMatcher TypeMatcher
        {
            get
            {
                EnsureTypeMatcherInitialized();
                return s_typeMatcher;
            }
        }

        /// <summary>
        /// Adds a fallback function that provides a default priority when no priority attribute is found.
        /// This will reset the type matcher to ensure newly added elements are sorted with the updated fallback.
        /// </summary>
        /// <param name="fallback">The fallback function to add.</param>
        public static void AddNullPriorityFallback(Func<Type, OrderPriority?> fallback)
        {
            NullPriorityFallbacks.Add(fallback);
            lock (InitializationLock)
            {
                s_typeMatcherInitialized = false;
                s_typeMatcher = null;
            }
        }

        private static void EnsureTypeMatcherInitialized()
        {
            if (s_typeMatcherInitialized) return;
            lock (InitializationLock)
            {
                if (s_typeMatcherInitialized) return;
                InitializeTypeMatcher();
                s_typeMatcherInitialized = true;
            }
        }

        private static void InitializeTypeMatcher()
        {
            if (s_elementTypes == null)
            {
                var elementTypes = new List<Type>();

                foreach (var type in AssemblyUtility.GetTypes(AssemblyCategory.Custom)
                             .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract))
                {
                    if (type.IsDerivedFrom<IHandler>())
                    {
                        elementTypes.Add(type);
                    }
                }
                s_elementTypes = elementTypes.ToArray();
            }

            s_typeMatcher = TypeMatcherFactory.CreateDefault();
            s_typeMatcher.SetTypeMatchCandidates(s_elementTypes
                .OrderByDescending(GetHandlerPriority)
                .Select((type, i) => new TypeMatchCandidate(type, s_elementTypes.Length - i, GetConstraints(type))));
        }

        public static Type[] GetConstraints(Type type)
        {
            // For generic inspector elements, extract the target generic type from the inheritance chain.
            // Example: For `SerializedDictionaryValueDrawer<TValue> : EasyValueDrawer<SerializedDictionary<string, TValue>>`,
            // `OpenGenericType` is `EasyValueDrawer<>` (the base class), and
            // `GetArgumentsOfInheritedOpenGenericType` returns `SerializedDictionary<string, TValue>`,
            // which is stored in `index.Targets` for later matching in `GetMatchedType`.
            Type currentType = type;
            do
            {
                if (currentType.IsDefined<HandlerConstraintsAttribute>())
                {
                    return currentType.GetGenericArguments();
                }
                currentType = currentType.BaseType;
            } while (currentType != null);

            if (type.BaseType == null || !type.BaseType.IsGenericType)
            {
                return null;
            }

            return type.GetGenericArgumentsRelativeTo(type.BaseType.GetGenericTypeDefinition());
        }

        public static Type GetFirstElementType(IElement element, Func<Type, bool> typeFilter = null, IList<Type[]> additionalMatchTypesList = null)
        {
            var results = GetHandlerTypeResults(element, additionalMatchTypesList);
            foreach (var result in results)
            {
                var type = result.MatchedType;
                if (typeFilter != null)
                {
                    if (!typeFilter(type))
                    {
                        continue;
                    }
                }

                if (!CanHandleElement(type, element))
                {
                    continue;
                }

                return type;
            }

            return null;
        }

        public static IEnumerable<Type> GetHandlerTypes(IElement element, Func<Type, bool> typeFilter = null, IList<Type[]> additionalMatchTypesList = null)
        {
            var results = GetHandlerTypeResults(element, additionalMatchTypesList);

            var set = new HashSet<Type>();
            foreach (var result in results)
            {
                if (set.Contains(result.MatchedType))
                {
                    continue;
                }

                var type = result.MatchedType;
                if (typeFilter != null)
                {
                    if (!typeFilter(type))
                    {
                        continue;
                    }
                }

                if (!CanHandleElement(type, element))
                {
                    continue;
                }

                set.Add(type);
                yield return type;
            }
        }

        private static TypeMatchResult[] GetHandlerTypeResults(IElement element, IList<Type[]> additionalMatchTypesList = null)
        {
            var resultsList = new List<TypeMatchResult[]>
            {
                TypeMatcher.GetMatches(Type.EmptyTypes),
            };

            // If the element is a value element, use its value type for matching
            if (element is IValueElement valueElement)
            {
                resultsList.Add(TypeMatcher.GetMatches(valueElement.ValueEntry.ValueType));
            }

            if (additionalMatchTypesList != null)
            {
                foreach (var matchTypes in additionalMatchTypesList)
                {
                    resultsList.Add(TypeMatcher.GetMatches(matchTypes));
                }
            }
            return TypeMatcher.GetMergedResults(resultsList);
        }

        private static OrderPriority GetHandlerPriority(Type handlerType)
        {
            OrderPriority? priority = null;

            if (handlerType.GetCustomAttributes(true)
                    .FirstOrDefault(attr => attr is IPriorityAccessor) is IPriorityAccessor priorityAttribute)
            {
                priority = priorityAttribute.Priority;
            }

            if (priority == null && NullPriorityFallbacks.Count > 0)
            {
                foreach (var fallback in NullPriorityFallbacks)
                {
                    priority = fallback(handlerType);
                    if (priority != null)
                    {
                        break;
                    }
                }
            }

            return priority ?? OrderPriority.Default;
        }

        /// <summary>
        /// Determines whether the specified handler type can handle the given element.
        /// Creates an instance of the handler type and calls its <see cref="IHandler.CanHandle(IElement)"/> method.
        /// </summary>
        /// <param name="handlerType">The handler type to test.</param>
        /// <param name="element">The inspector element to check.</param>
        /// <returns>True if the handler can handle the element; otherwise, false.</returns>
        private static bool CanHandleElement(Type handlerType, IElement element)
        {
            var handler = (IHandler)FormatterServices.GetUninitializedObject(handlerType);
            return handler.CanHandle(element);
        }
    }
}
