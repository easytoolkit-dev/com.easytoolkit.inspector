using System;
using System.Diagnostics;
using EasyToolkit.Inspector.Attributes;

[assembly: RegisterGroupAttributeScope(typeof(FoldoutGroupAttribute), typeof(EndFoldoutGroupAttribute))]

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class FoldoutGroupAttribute : BeginGroupAttribute
    {
        public string Label { get; set; }
        public bool? Expanded { get; set; }

        public override string GroupName => GroupCatalogue + "/" + Label;

        public FoldoutGroupAttribute(string label)
        {
            Label = label;
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class EndFoldoutGroupAttribute : EndGroupAttribute
    {
    }
}
