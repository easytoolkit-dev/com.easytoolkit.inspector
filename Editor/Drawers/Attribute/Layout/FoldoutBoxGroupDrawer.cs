using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class FoldoutBoxGroupDrawer : EasyGroupDrawer<FoldoutBoxGroupAttribute>
    {
        private IExpressionEvaluator _labelEvaluator;

        protected override void Initialize()
        {
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

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

            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label)
        {
            EasyEditorGUI.BeginBox();
            EasyEditorGUI.BeginBoxHeader();
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);
            var labelText = _labelEvaluator.Evaluate<string>(resolveTarget);
            Element.State.Expanded = EasyEditorGUI.Foldout(Element.State.Expanded, EditorHelper.TempContent(labelText));
            EasyEditorGUI.EndBoxHeader();
        }

        protected override void EndDrawGroup()
        {
            EasyEditorGUI.EndBox();
        }
    }
}
