using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method parameter definition implementation handling individual parameters for method invocation.
    /// Similar to dynamically created custom values, representing individual parameter items for method invocation.
    /// </summary>
    public sealed class MethodParameterDefinition : ValueDefinition, IMethodParameterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodParameterDefinition"/> class.
        /// </summary>
        /// <param name="parameterInfo">The <see cref="ParameterInfo"/> that describes the method parameter.</param>
        /// <param name="parameterIndex">The index of this parameter in the method's parameter list.</param>
        /// <param name="name">The element name. Uses the parameter name when null.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="parameterInfo"/> is null.</exception>
        public MethodParameterDefinition(
            ParameterInfo parameterInfo,
            int parameterIndex,
            string name = null,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(
                (parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo))).ParameterType,
                ElementRoles.MethodParameter,
                name ?? parameterInfo.Name,
                additionalAttributes)
        {
            ParameterInfo = parameterInfo;
            ParameterIndex = parameterIndex;
        }

        /// <summary>
        /// Gets the index of this parameter in the method's parameter list.
        /// </summary>
        public int ParameterIndex { get; }

        /// <summary>
        /// Gets the <see cref="ParameterInfo"/> that describes the method parameter.
        /// </summary>
        public ParameterInfo ParameterInfo { get; }
    }
}
