using System;
using System.Diagnostics;

namespace EasyToolkit.Inspector.Attributes
{
    /// <summary>
    /// Groups fields or properties into a collapsible foldout box in the Inspector.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class FoldoutBoxGroupAttribute : GroupAttribute
    {
        /// <summary>
        /// Gets or sets the display label for the foldout group header.
        /// </summary>
        public string Label { get; set; }

        // Nullable backing field: null means Expanded is not explicitly set,
        // allowing the drawer to use its default behavior.
        private bool? _expanded;

        /// <summary>
        /// Gets or sets the initial expanded state of the foldout group.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when getting this property without first setting a value.
        /// Check <see cref="IsDefinedExpanded"/> before accessing.
        /// </exception>
        /// <remarks>
        /// This property is optional. When not set, the drawer uses its default behavior.
        /// Always verify <see cref="IsDefinedExpanded"/> is true before accessing this property.
        /// </remarks>
        public bool Expanded
        {
            get => _expanded ?? throw new InvalidOperationException(
                "Cannot access Expanded property because it has not been set. " +
                "Check IsDefinedExpanded before accessing Expanded, or set a value first.");
            set => _expanded = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Expanded"/> property has been explicitly defined.
        /// </summary>
        /// <remarks>
        /// Used by the Drawer backend to determine whether
        /// to use the explicit expanded state or default behavior.
        /// </remarks>
        public bool IsDefinedExpanded => _expanded.HasValue;

        /// <summary>
        /// Gets the full group name path for this foldout group.
        /// </summary>
        public override string GroupName => GroupCatalogue + "/" + Label;

        /// <summary>
        /// Initializes a new instance of the <see cref="FoldoutBoxGroupAttribute"/> class.
        /// </summary>
        /// <param name="label">The display label for the foldout group header.</param>
        public FoldoutBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }
}
