using System.Reflection;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    public interface IMemberDefinition
    {
        MemberInfo MemberInfo { get; }
    }
}
