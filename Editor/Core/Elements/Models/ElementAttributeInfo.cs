using System;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    public class ElementAttributeInfo
    {
        public ElementAttributeInfo(Attribute attribute, ElementAttributeSource source)
        {
            Attribute = attribute;
            Source = source;
        }

        public Attribute Attribute { get; }
        public ElementAttributeSource Source { get; }
    }
}
