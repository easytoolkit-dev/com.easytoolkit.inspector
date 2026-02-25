using System;
using System.Reflection;
using EasyToolkit.Core;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Core.Unity;
using EasyToolkit.Inspector.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Enhanced Unity editor that provides property tree-based inspector functionality
    /// with support for audio filter GUI integration and custom inspector drawing.
    /// </summary>
    [CanEditMultipleObjects]
    public class EasyEditor : UnityEditor.Editor
    {
        // Static delegates for Unity's internal audio filter functionality
        private static StaticInvoker<MonoBehaviour, int> s_getCustomFilterChannelCount;
        private static InstanceVoidInvoker<object, MonoBehaviour> s_drawAudioFilterGUI;

        // Reflection state tracking
        private static bool s_hasReflectedAudioFilter;
        private static bool s_initialized;
        private static Type s_audioFilterGUIType;

        // Instance fields for property tree and audio filter GUI
        private IElementTree _tree;
        private object _audioFilterGUIInstance;
        private EasyInspectorAttribute _inspectorAttribute;

        public IElementTree Tree => _tree;

        /// <summary>
        /// Gets or sets whether this editor is being used as an inline editor.
        /// When true, the MonoScript field will not be drawn.
        /// </summary>
        public bool IsInlineEditor { get; set; }

        /// <summary>
        /// Called by Unity to draw the inspector GUI.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawIMGUI();
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (_inspectorAttribute.BackendMode != InspectorBackendMode.UIToolkit)
            {
                return null;
            }

            var root = new VisualElement();

            try
            {
                _tree = InspectorElements.TreeFactory.CreateTree(serializedObject, InspectorBackendMode.UIToolkit, root);
            }
            catch (ArgumentException e)
            {
                Debug.LogException(e);
                return null;
            }

            return root;
        }

        /// <summary>
        /// Called when the editor is enabled.
        /// Ensures the static initialization is performed.
        /// </summary>
        protected virtual void OnEnable()
        {
            EnsureInitialized();
            _inspectorAttribute = target.GetType().GetCustomAttribute<EasyInspectorAttribute>();

            EditorApplication.update += DrawUIToolkit;
        }

        /// <summary>
        /// Called when the editor is disabled.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (_tree != null)
            {
                (_tree as IDisposable)?.Dispose();
                _tree = null;
            }
            EditorApplication.update -= DrawUIToolkit;
        }

        /// <summary>
        /// Ensures the static initialization is performed once.
        /// Uses reflection to access Unity's internal audio filter functionality.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!s_initialized)
            {
                s_initialized = true;

                try
                {
                    // Get the AudioUtil type
                    var audioUtilType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioUtil");

                    s_getCustomFilterChannelCount = ReflectionCompiler.CreateStaticMethodInvoker<MonoBehaviour, int>(
                        audioUtilType.GetMethod("GetCustomFilterChannelCount", MemberAccessFlags.AllStatic));

                    // Get the internal AudioFilterGUI type and create a delegate for its DrawAudioFilterGUI method
                    s_audioFilterGUIType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.AudioFilterGUI");

                    // Wrap the generic invoker with a strongly-typed delegate
                    s_drawAudioFilterGUI = ReflectionCompiler.CreateInstanceVoidMethodInvoker<object, MonoBehaviour>(
                        s_audioFilterGUIType.GetMethod("DrawAudioFilterGUI",
                            BindingFlags.Public | BindingFlags.Instance));

                    s_hasReflectedAudioFilter = true;
                }
                catch (Exception e)
                {
                    // Log a warning if reflection fails due to Unity internal changes
                    Debug.LogWarning(
                        "The internal Unity class AudioFilterGUI has been changed; cannot properly mock a generic Unity inspector. This probably won't be very noticeable.");
                }
            }
        }
        private static bool HasAudioCallback(MonoBehaviour behaviour)
        {
            return behaviour.GetType().GetMethod(
                "OnAudioFilterRead",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            ) != null;
        }

        /// <summary>
        /// Main method that draws the inspector GUI.
        /// Handles property tree drawing and audio filter GUI integration.
        /// </summary>
        private void DrawIMGUI()
        {
            // Fall back to default inspector if property tree creation failed
            if (_tree == null)
            {
                try
                {
                    _tree = InspectorElements.TreeFactory.CreateTree(serializedObject);
                }
                catch (ArgumentException e)
                {
                    Debug.LogException(e);
                }
                base.OnInspectorGUI();
                return;
            }

            // Configure MonoScript field visibility during layout phase
            if (Event.current.type == EventType.Layout)
            {
                _tree.DrawMonoScriptObjectField = _tree.SerializedObject != null &&
                                                  _tree.TargetType != null &&
                                                  InspectorConfigAsset.Instance.DrawMonoScriptInEditor &&
                                                  !IsInlineEditor;
            }

            // Draw the inspector within localization context
            using (new LocalizationGroup(target))
            {
                // Draw the main property tree
                DrawTree();

                var targetBehaviour = target as MonoBehaviour;
                // Draw audio filter GUI if the target is a MonoBehaviour with audio callbacks
                if (s_hasReflectedAudioFilter && targetBehaviour != null)
                {
                    if (HasAudioCallback(targetBehaviour) &&
                        s_getCustomFilterChannelCount(targetBehaviour) > 0)
                    {
                        // Create audio filter GUI instance if it doesn't exist
                        if (this._audioFilterGUIInstance == null)
                        {
                            this._audioFilterGUIInstance = Activator.CreateInstance(s_audioFilterGUIType);
                        }

                        // Draw the audio filter GUI using the reflected delegate
                        s_drawAudioFilterGUI(ref _audioFilterGUIInstance, targetBehaviour);
                    }
                }
            }
        }

        private void DrawUIToolkit()
        {
            if (_tree != null)
            {
                DrawTree();
            }
        }

        /// <summary>
        /// Draws the property tree.
        /// Can be overridden by derived classes to customize tree drawing behavior.
        /// </summary>
        protected virtual void DrawTree()
        {
            _tree.Draw();
        }
    }
}
