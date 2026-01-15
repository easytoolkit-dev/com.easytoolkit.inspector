using EasyToolKit.Core.Textual;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
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
                _buttonLabelEvaluator = ExpressionEvaluatorFactory
                    .Evaluate(Attribute.Label, targetType)
                    .WithExpressionFlag()
                    .Build();
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
