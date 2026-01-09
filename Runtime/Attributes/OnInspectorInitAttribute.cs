using System;

namespace EasyToolKit.Inspector.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class OnInspectorInitAttribute : MethodAttribute
    {
    }
}
