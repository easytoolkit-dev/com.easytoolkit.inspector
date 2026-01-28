using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class LabelTextAttributeDrawer : EasyAttributeDrawer<LabelTextAttribute>
    {
        private IExpressionEvaluator _labelEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _labelEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                Attribute.Label, targetType, requireExpressionFlag: true);
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            label.text = _labelEvaluator.Evaluate<string>(resolveTarget);
            CallNextDrawer(label);
        }
    }
}
