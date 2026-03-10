using System;
using System.Diagnostics;

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class FoldoutBoxGroupAttribute : GroupAttribute
    {
        public string Label { get; set; }
        public bool? Expanded { get; set; }

        public override string GroupName => GroupCatalogue + "/" + Label;

        public FoldoutBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }
}
