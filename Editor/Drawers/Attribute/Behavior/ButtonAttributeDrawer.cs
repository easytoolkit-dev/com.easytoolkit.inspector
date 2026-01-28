using EasyToolkit.Core.Textual;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    //TODO support parameters
    public class ButtonAttributeDrawer : EasyMethodAttributeDrawer<ButtonAttribute>
    {
        [CanBeNull] private IExpressionEvaluator _buttonLabelEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            if (Attribute.Label.IsNotNullOrEmpty())
            {
                _buttonLabelEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                    Attribute.Label, targetType, requireExpressionFlag: true);
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_buttonLabelEvaluator != null && _buttonLabelEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var buttonLabel = _buttonLabelEvaluator != null
                ? _buttonLabelEvaluator.Evaluate<string>(resolveTarget)
                : label.text;
            if (GUILayout.Button(buttonLabel))
            {
                foreach (var target in Element.LogicalParent.CastValue().ValueEntry.EnumerateWeakValues())
                {
                    if (target == null)
                        continue;
                    MethodInfo.Invoke(target, null);
                }
            }
        }
    }
}
