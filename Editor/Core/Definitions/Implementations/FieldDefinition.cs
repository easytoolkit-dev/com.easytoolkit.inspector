using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Field definition implementation that handles <see cref="System.Reflection.FieldInfo"/>.
    /// Provides consistent access to field reflection information.
    /// </summary>
    public sealed class FieldDefinition : ValueDefinition, IFieldDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinition"/> class.
        /// </summary>
        /// <param name="fieldInfo">The <see cref="System.Reflection.FieldInfo"/> that represents this field.</param>
        /// <param name="asUnityProperty">Whether this field should be treated as a Unity property.</param>
        /// <param name="name">The element name. Uses the field name when null.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldInfo"/> is null.</exception>
        public FieldDefinition(
            FieldInfo fieldInfo,
            bool asUnityProperty = false,
            string name = null,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(
                (fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo))).FieldType,
                ElementRoles.Field,
                name ?? fieldInfo.Name,
                additionalAttributes)
        {
            FieldInfo = fieldInfo;
            AsUnityProperty = asUnityProperty;
        }

        /// <summary>
        /// Gets the <see cref="System.Reflection.FieldInfo"/> that represents this field.
        /// </summary>
        public FieldInfo FieldInfo { get; }

        /// <summary>
        /// Gets whether this field should be treated as a Unity property.
        /// </summary>
        public bool AsUnityProperty { get; }

        /// <inheritdoc/>
        public MemberInfo MemberInfo => FieldInfo;
    }
}
