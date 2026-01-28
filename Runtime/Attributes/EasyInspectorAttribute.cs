using System;
using System.Diagnostics;

namespace EasyToolkit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class EasyInspectorAttribute : InspectorAttribute
    {
    }
}
