using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class ShowInInspectorAttribute : InspectorAttribute
    {
    }
}
