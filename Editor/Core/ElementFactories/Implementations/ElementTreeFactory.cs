using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor.Implementations
{
    /// <summary>
    /// Default factory implementation for creating <see cref="IElementTree"/> instances.
    /// </summary>
    public class ElementTreeFactory : IElementTreeFactory
    {
        /// <inheritdoc/>
        public IElementTree CreateTree(SerializedObject serializedObject, InspectorBackendMode backendMode,
            VisualElement rootVisualElement)
        {
            if (serializedObject == null)
                throw new ArgumentNullException(nameof(serializedObject));

            var targets = serializedObject.targetObjects.Cast<object>().ToArray();
            return CreateTree(targets, serializedObject, backendMode, rootVisualElement);
        }

        /// <inheritdoc/>
        public IElementTree CreateTree(object[] targets, SerializedObject serializedObject,
            InspectorBackendMode backendMode, VisualElement rootVisualElement)
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

            return new ElementTree(targets, serializedObject, backendMode, rootVisualElement);
        }
    }
}
