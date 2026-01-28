using System;

namespace EasyToolkit.Inspector.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnInspectorInitAttribute : MethodAttribute
    {
    }
}
