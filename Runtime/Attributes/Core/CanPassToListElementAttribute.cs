using System;

namespace EasyToolKit.Inspector.Attributes
{
    public abstract class CanPassToListElementAttribute : InspectorAttribute
    {
        public bool PassToListElements { get; set; }
    }
}
