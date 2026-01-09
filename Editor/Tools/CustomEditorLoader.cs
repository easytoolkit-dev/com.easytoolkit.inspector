using UnityEditor.Callbacks;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    internal static class CustomEditorLoader
    {
        [DidReloadScripts]
        static CustomEditorLoader()
        {
            InspectorConfigAsset.Instance.UpdateEditors();
        }
    }
}
