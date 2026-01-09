using System;
using System.Diagnostics;
using EasyToolKit.Inspector.Attributes;

[assembly: RegisterGroupAttributeScope(typeof(FoldoutBoxGroupAttribute), typeof(EndFoldoutBoxGroupAttribute))]

namespace EasyToolKit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class FoldoutBoxGroupAttribute : BeginGroupAttribute
    {
        public string Label { get; set; }
        public bool? Expanded { get; set; }

        public override string GroupName => GroupCatalogue + "/" + Label;

        public FoldoutBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class EndFoldoutBoxGroupAttribute : EndGroupAttribute
    {
    }
}
