using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Factory for creating <see cref="IElementTree"/> instances.
    /// </summary>
    public static class ElementTreeFactory
    {
        /// <summary>
        /// Creates an element tree for a single serialized object.
        /// </summary>
        /// <param name="serializedObject">The serialized object to create a tree for.</param>
        /// <param name="backendMode">The backend rendering mode used by the tree.</param>
        /// <param name="rootVisualElement">The optional root visual element for UI Toolkit rendering.</param>
        /// <returns>A new <see cref="IElementTree"/> instance representing the complete inspector element hierarchy.</returns>
        [NotNull]
        public static IElementTree CreateTree(
            [NotNull] SerializedObject serializedObject,
            InspectorBackendMode backendMode = InspectorBackendMode.IMGUI,
            [CanBeNull] VisualElement rootVisualElement = null)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            var targets = serializedObject.targetObjects.Cast<object>().ToArray();
            return CreateTree(targets, serializedObject, backendMode, rootVisualElement);
        }

        /// <summary>
        /// Creates an element tree for multiple target objects with an optional serialized object.
        /// </summary>
        /// <param name="targets">The target objects to create a tree for.</param>
        /// <param name="serializedObject">The optional serialized object containing shared serialized data.</param>
        /// <param name="backendMode">The backend rendering mode used by the tree.</param>
        /// <param name="rootVisualElement">The optional root visual element for UI Toolkit rendering.</param>
        /// <returns>A new <see cref="IElementTree"/> instance representing the multi-object inspector element hierarchy.</returns>
        /// <remarks>
        /// If <paramref name="serializedObject"/> is not null, all objects in <paramref name="targets"/>
        /// must match the objects in <see cref="SerializedObject.targetObjects"/>.
        /// </remarks>
        [NotNull]
        public static IElementTree CreateTree(
            [NotNull] object[] targets,
            [CanBeNull] SerializedObject serializedObject,
            InspectorBackendMode backendMode = InspectorBackendMode.IMGUI,
            [CanBeNull] VisualElement rootVisualElement = null)
        {
            if (targets == null)
                throw new ArgumentNullException(nameof(targets));

            if (serializedObject != null)
            {
                bool valid = true;
                var targetObjects = serializedObject.targetObjects;

                if (targets.Length != targetObjects.Length)
                {
                    valid = false;
                }
                else
                {
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (!object.ReferenceEquals(targets[i], targetObjects[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    throw new ArgumentException($"SerializedObject is not valid for targets.");
                }
            }
            else
            {
                // Check if all targets have the same type
                if (targets.Length > 0)
                {
                    Type firstTargetType = targets[0].GetType();
                    bool allSameType = targets.All(t => t.GetType() == firstTargetType);

                    if (!allSameType)
                    {
                        throw new ArgumentException($"All targets must have the same type.");
                    }

                    // Check if the type inherits from UnityEngine.Object
                    if (typeof(UnityEngine.Object).IsAssignableFrom(firstTargetType))
                    {
                        // Convert targets to UnityEngine.Object array
                        var unityObjects = targets.Cast<UnityEngine.Object>().ToArray();
                        serializedObject = new SerializedObject(unityObjects);
                    }
                }
            }

            return new Implementations.ElementTree(targets, serializedObject, backendMode, rootVisualElement);
        }
    }
}
