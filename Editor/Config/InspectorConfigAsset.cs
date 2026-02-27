using System;
using EasyToolkit.Core;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Editor.Internal;
using UnityEditor;
using UnityEngine;
using System.Linq;
using EasyToolkit.Core.Patterns;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;

namespace EasyToolkit.Inspector.Editor
{
    [ScriptableObjectSingletonConfiguration("Plugins/EasyToolKit/Inspector/Editor/Configs", ScriptableObjectLoadMode.Asset)]
    public class InspectorConfigAsset : ScriptableObjectSingleton<InspectorConfigAsset>, ISerializationCallbackReceiver
    {
        [SerializeField] private bool _drawMonoScriptInEditor = true;
        [SerializeField] private bool _instantiateReferenceObjectIfNull = true;

        public bool DrawMonoScriptInEditor => _drawMonoScriptInEditor;
        public bool TryInstantiateReferenceObjectIfNull => _instantiateReferenceObjectIfNull;

        private bool _hasUpdatedEditorsOnce;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            UnityEditorEventUtility.DelayAction(UpdateEditors);
        }

        public void UpdateEditors()
        {
            var drawnTypes = AssemblyUtility.GetTypes(AssemblyCategory.Custom)
                .Where(type => type.IsDefined<EasyInspectorAttribute>(inherit: true, includeInterface: true))
                .Where(type => type.IsSubclassOf(typeof(Component)) ||
                               type.IsSubclassOf(typeof(ScriptableObject)));

            foreach (var drawnType in drawnTypes)
            {
                CustomEditorUtility.SetCustomEditor(drawnType, typeof(EasyEditor), false, false);
            }

            EditorApplication.delayCall += () =>
            {
                Type inspectorWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.InspectorWindow");
                Type activeEditorTrackerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ActiveEditorTracker");

                if (inspectorWindowType != null && activeEditorTrackerType != null)
                {
                    var createTrackerMethod =
                        inspectorWindowType.GetMethod("CreateTracker", MemberAccessFlags.AllInstance);
                    var trackerField = inspectorWindowType.GetField("m_Tracker", MemberAccessFlags.AllInstance);
                    var forceRebuild =
                        activeEditorTrackerType.GetMethod("ForceRebuild", MemberAccessFlags.AllInstance);

                    if (createTrackerMethod != null && trackerField != null && forceRebuild != null)
                    {
                        // 获取所有检查器窗口并强制重建
                        var windows = Resources.FindObjectsOfTypeAll(inspectorWindowType);

                        foreach (var window in windows)
                        {
                            createTrackerMethod.Invoke(window, null);
                            object tracker = trackerField.GetValue(window);
                            forceRebuild.Invoke(tracker, null);
                        }
                    }
                }
            };
            _hasUpdatedEditorsOnce = true;
        }

        public void EnsureEditorsHaveBeenUpdated()
        {
            if (!_hasUpdatedEditorsOnce)
            {
                UpdateEditors();
                _hasUpdatedEditorsOnce = true;
            }
        }
    }
}
