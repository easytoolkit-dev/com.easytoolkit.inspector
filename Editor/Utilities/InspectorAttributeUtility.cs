using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyToolkit.Core;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using JetBrains.Annotations;

namespace EasyToolkit.Inspector.Editor
{
    public static class InspectorAttributeUtility
    {
        private static readonly RegisterGroupAttributeScopeAttribute[] _groupAttributeScopes;

        private static readonly Dictionary<Type, Type> EndGroupAttributeTypeCache =
            new Dictionary<Type, Type>();

        private static readonly Dictionary<Type, Type> BeginGroupAttributeTypeCache =
            new Dictionary<Type, Type>();

        static InspectorAttributeUtility()
        {
            _groupAttributeScopes = AssemblyUtility.GetAllAssemblies()
                .SelectMany(asm => asm.GetCustomAttributes<RegisterGroupAttributeScopeAttribute>())
                .ToArray();
        }

        public static Type GetCorrespondGroupAttributeType([NotNull] Type groupAttributeType)
        {
            if (groupAttributeType == null)
                throw new ArgumentNullException(nameof(groupAttributeType));

            if (groupAttributeType.IsDerivedFrom<BeginGroupAttribute>())
            {
                return GetCorrespondEndGroupAttributeType(groupAttributeType);
            }

            if (groupAttributeType.IsDerivedFrom<EndGroupAttribute>())
            {
                return GetCorrespondStartGroupAttributeType(groupAttributeType);
            }

            throw new ArgumentException($"Invalid group attribute type: {groupAttributeType}");
        }

        private static Type GetCorrespondEndGroupAttributeType(Type beginGroupAttributeType)
        {
            if (EndGroupAttributeTypeCache.TryGetValue(beginGroupAttributeType, out var type))
            {
                return type;
            }

            var scope = _groupAttributeScopes.FirstOrDefault(scope => scope.BeginGroupAttributeType == beginGroupAttributeType);
            if (scope == null)
            {
                throw new InvalidOperationException($"Invalid begin group attribute type: {beginGroupAttributeType}");
            }
            type = scope.EndGroupAttributeType;
            EndGroupAttributeTypeCache[beginGroupAttributeType] = type;
            return type;
        }

        private static Type GetCorrespondStartGroupAttributeType(Type endGroupAttributeType)
        {
            if (BeginGroupAttributeTypeCache.TryGetValue(endGroupAttributeType, out var type))
            {
                return type;
            }

            var scope = _groupAttributeScopes.FirstOrDefault(scope => scope.EndGroupAttributeType == endGroupAttributeType);
            if (scope == null)
            {
                throw new InvalidOperationException($"Invalid end group attribute type: {endGroupAttributeType}");
            }
            type = scope.BeginGroupAttributeType;
            BeginGroupAttributeTypeCache[endGroupAttributeType] = type;
            return type;
        }
    }
}
