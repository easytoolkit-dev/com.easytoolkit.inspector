using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector.Attributes
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
