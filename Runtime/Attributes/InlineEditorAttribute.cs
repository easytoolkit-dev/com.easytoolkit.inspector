using System;
using System.Diagnostics;

namespace EasyToolkit.Inspector.Attributes
{
    public enum InlineEditorStyle
    {
        Place,
        PlaceWithHide,
        Box,
        Foldout,
        FoldoutBox,
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class InlineEditorAttribute : InspectorAttribute
    {
        public InlineEditorStyle Style { get; set; }
    }
}
