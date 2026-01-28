using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 10)]
    public class TitleAttributeDrawer : EasyAttributeDrawer<TitleAttribute>
    {
        private IExpressionEvaluator _titleEvaluator;
        private IExpressionEvaluator _subtitleEvaluator;

        protected override void Initialize()
        {
            var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);

            _titleEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                Attribute.Title, targetType, requireExpressionFlag: true);
            _subtitleEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                Attribute.Subtitle, targetType, requireExpressionFlag: true);
        }

        protected override void Draw(GUIContent label)
        {
            if (_titleEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }
            if (_subtitleEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var resolveTarget = ElementUtility.GetOwnerWithAttribute(Element, Attribute);
            var titleText = _titleEvaluator.Evaluate<string>(resolveTarget);
            var subtitleText = _subtitleEvaluator.Evaluate<string>(resolveTarget);
            EasyEditorGUI.Title(titleText, subtitleText, Attribute.TextAlignment, Attribute.HorizontalLine, Attribute.BoldTitle);

            CallNextDrawer(label);
        }
    }
}
