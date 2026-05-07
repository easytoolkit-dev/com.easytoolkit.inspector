using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Field collection definition implementation that unifies <see cref="ICollectionDefinition"/> and <see cref="IFieldDefinition"/>.
    /// Represents collection fields on an object, providing both collection-specific metadata and reflection information.
    /// </summary>
    public sealed class FieldCollectionDefinition : CollectionDefinition, IFieldCollectionDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldCollectionDefinition"/> class.
        /// </summary>
        /// <param name="fieldInfo">The <see cref="System.Reflection.FieldInfo"/> that represents this field.</param>
        /// <param name="itemType">The type of elements contained in this collection.</param>
        /// <param name="isOrdered">Whether this collection is ordered.</param>
        /// <param name="asUnityProperty">Whether this field should be treated as a Unity property.</param>
        /// <param name="name">The element name. Uses the field name when null.</param>
        /// <param name="additionalAttributes">Additional attributes applied to the element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fieldInfo"/> is null.</exception>
        public FieldCollectionDefinition(
            FieldInfo fieldInfo,
            Type itemType,
            bool isOrdered,
            bool asUnityProperty = false,
            string name = null,
            IReadOnlyList<Attribute> additionalAttributes = null)
            : base(
                (fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo))).FieldType,
                itemType,
                isOrdered,
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
