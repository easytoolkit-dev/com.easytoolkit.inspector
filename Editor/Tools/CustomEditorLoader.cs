using UnityEditor;
using UnityEditor.Callbacks;

namespace EasyToolkit.Inspector.Editor
{
    internal static class CustomEditorLoader
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            EditorApplication.delayCall += () =>
            {
                InspectorConfigAsset.Instance.UpdateEditors();
            };
        }
    }
}
