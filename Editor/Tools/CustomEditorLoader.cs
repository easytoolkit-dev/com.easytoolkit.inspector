using UnityEditor.Callbacks;

namespace EasyToolkit.Inspector.Editor
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
