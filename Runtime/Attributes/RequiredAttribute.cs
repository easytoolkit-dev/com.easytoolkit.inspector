using System;

namespace EasyToolkit.Inspector.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : CanPassToListElementAttribute
    {
    }
}
