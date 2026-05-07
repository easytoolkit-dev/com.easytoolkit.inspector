using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Method definition implementation for function handling in the inspector.
    /// Provides metadata for methods that can be invoked or displayed in the inspector interface.
    /// </summary>
    public sealed class MethodDefinition : ElementDefinition, IMethodDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodDefinition"/> class.
        /// </summary>
        /// <param name="methodInfo">The <see cref="MethodInfo"/> that describes the method.</param>
        /// <param name="name">The element name. Uses the method name when null.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="methodInfo"/> is null.</exception>
        public MethodDefinition(
            MethodInfo methodInfo,
            string name = null,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(
                ElementRoles.Method,
                name ?? (methodInfo ?? throw new ArgumentNullException(nameof(methodInfo))).Name,
                additionalAttributes)
        {
            MethodInfo = methodInfo;
        }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> that describes the method.
        /// </summary>
        public MethodInfo MethodInfo { get; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => MethodInfo;
    }
}
