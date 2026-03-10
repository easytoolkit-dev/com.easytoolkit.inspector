using System;
using System.Diagnostics;

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MetroBoxGroupAttribute : GroupAttribute
    {
        public string Label { get; set; }
        public string IconTextureGetter { get; set; }

        public override string GroupName => GroupCatalogue + "/" + Label;

        public MetroBoxGroupAttribute(string label)
        {
            Label = label;
        }
    }
}
