using System;

namespace EasyToolkit.Inspector.Attributes
{
    public abstract class GroupAttribute : InspectorAttribute
    {
        public string GroupCatalogue { get; set; }
        public bool EndAfterThisProperty { get; set; }

        public virtual string GroupName => GroupCatalogue;

        protected GroupAttribute()
        {
        }
    }
}
