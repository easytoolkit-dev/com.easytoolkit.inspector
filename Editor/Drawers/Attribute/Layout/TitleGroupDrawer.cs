using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// Drawer for <see cref="TitleGroupAttribute"/> that renders a titled section group.
    /// </summary>
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class TitleGroupDrawer : EasyGroupDrawer<TitleGroupAttribute>
    {
        private IExpressionEvaluator _titleEvaluator;
        private IExpressionEvaluator _subtitleEvaluator;

        protected override void Initialize()
        {
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

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

            Element.State.Expanded = true;
            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label)
        {
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);
            var titleText = _titleEvaluator.Evaluate<string>(resolveTarget);
            var subtitleText = _subtitleEvaluator.Evaluate<string>(resolveTarget);

            EasyEditorGUI.Title(titleText, subtitleText, Attribute.TextAlignment, Attribute.HorizontalLine, Attribute.BoldTitle);
        }
    }
}
