using System;

namespace EasyToolkit.Inspector.Editor
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class HandlerConstraintsAttribute : Attribute
    {
        public HandlerConstraintsAttribute()
        {
        }
    }
}
