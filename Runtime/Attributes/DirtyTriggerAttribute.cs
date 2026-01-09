using System;
using System.Diagnostics;

namespace EasyToolKit.Inspector.Attributes
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DirtyTriggerAttribute : AuxiliaryAttribute
    {
        public DirtyTriggerAttribute()
        {
        }
    }
}
