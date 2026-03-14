using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolkit.Inspector.Attributes
{
    /// <summary>
    /// Groups fields or properties into a collapsible foldout section with Metro-style UI in the Inspector.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetroFoldoutGroupAttribute : GroupAttribute
    {
        /// <summary>
        /// Gets or sets the display label for the foldout group header.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label displayed on the right side of the header.
        /// </summary>
        public string RightLabel { get; set; }

        /// <summary>
        /// Gets or sets the tooltip text shown when hovering over the header.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Gets or sets the member variable/function name or expression for the right label color.
        /// </summary>
        public string RightLabelColorGetter { get; set; }

        /// <summary>
        /// Gets or sets the member variable/function name or expression for the side line color.
        /// </summary>
        public string SideLineColorGetter { get; set; }

        /// <summary>
        /// Gets or sets the member variable/function name or expression for the icon texture.
        /// </summary>
        public string IconTextureGetter { get; set; }

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
        /// Initializes a new instance of the <see cref="MetroFoldoutGroupAttribute"/> class.
        /// </summary>
        /// <param name="label">The display label for the foldout group header.</param>
        /// <param name="tooltip">The optional tooltip text shown when hovering over the header.</param>
        public MetroFoldoutGroupAttribute(string label, string tooltip = null)
        {
            Label = label;
            Tooltip = tooltip;
        }
    }
}
