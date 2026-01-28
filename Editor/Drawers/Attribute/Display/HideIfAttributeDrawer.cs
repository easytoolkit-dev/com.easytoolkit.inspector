using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super + 10)]
    public class HideIfAttributeDrawer : EasyAttributeDrawer<HideIfAttribute>
    {
        private IExpressionEvaluator _conditionEvaluator;
        private bool _hide;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _conditionEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                Attribute.Condition, targetType);
        }

        protected override void Draw(GUIContent label)
        {
            if (_conditionEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Event.current.type == EventType.Layout)
            {
                var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
                var condition = _conditionEvaluator.Evaluate(resolveTarget);
                var value = Attribute.Value;
                _hide = Equals(condition, value);
            }

            if (!_hide)
            {
                CallNextDrawer(label);
            }
        }
    }
}
