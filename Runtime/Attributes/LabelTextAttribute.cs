using System;
using System.Diagnostics;

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class LabelTextAttribute : InspectorAttribute
    {
        public string Label { get; set; }

        public LabelTextAttribute(string label)
        {
            Label = label;
        }
    }
}
