using System;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    /// <summary>
    /// Interface for resolving value structure information in the inspector system.
    /// Focuses purely on value structure without collection operations or change management.
    /// </summary>
    public interface IStructureResolver : IResolver
    {
        IElementDefinition[] GetChildrenDefinitions();
    }
}
