using System;
using System.Diagnostics;
using UnityEngine;

namespace EasyToolkit.Inspector.Attributes
{
    /// <summary>
    /// Groups inspector fields under a titled section with optional subtitle and horizontal line.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TitleGroupAttribute : GroupAttribute
    {
        /// <summary>
        /// Gets or sets the main title text displayed at the top of the group.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets whether the title should be rendered in bold.
        /// </summary>
        public bool BoldTitle { get; set; } = true;

        /// <summary>
        /// Gets or sets the optional subtitle text displayed below the title.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets whether a horizontal line should be drawn below the title/subtitle.
        /// </summary>
        public bool HorizontalLine { get; set; } = true;

        /// <summary>
        /// Gets or sets the text alignment for the title and subtitle.
        /// </summary>
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;

        /// <summary>
        /// Gets the unique group name for this title group.
        /// </summary>
        public override string GroupName => GroupCatalogue + "/" + Title;

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleGroupAttribute"/> class.
        /// </summary>
        /// <param name="title">The title text to display.</param>
        public TitleGroupAttribute(string title)
        {
            Title = title;
        }
    }
}
