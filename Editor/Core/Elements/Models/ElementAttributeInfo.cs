using System;

namespace EasyToolkit.Inspector.Editor
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
