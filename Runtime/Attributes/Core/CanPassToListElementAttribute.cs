using System;

namespace EasyToolkit.Inspector.Attributes
{
    public abstract class CanPassToListElementAttribute : InspectorAttribute
    {
        public bool PassToListElements { get; set; }
    }
}
