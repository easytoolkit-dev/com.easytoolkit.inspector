using System.Reflection;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Base implementation of <see cref="IMemberDefinition"/> for members with reflection information.
    /// </summary>
    public abstract class MemberDefinition : IMemberDefinition
    {
        /// <summary>
        /// Gets or sets the reflection information about the member.
        /// </summary>
        public MemberInfo MemberInfo { get; set; }
    }
}
