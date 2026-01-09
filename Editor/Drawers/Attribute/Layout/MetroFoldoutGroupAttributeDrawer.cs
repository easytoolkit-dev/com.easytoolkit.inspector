using EasyToolKit.Core.Common;
using EasyToolKit.Core.Editor;
using EasyToolKit.Core.Mathematics;
using EasyToolKit.Core.Reflection;
using EasyToolKit.Inspector.Attributes.Editor.Internal;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Attributes.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class MetroFoldoutGroupAttributeDrawer : EasyGroupAttributeDrawer<MetroFoldoutGroupAttribute>
    {
        private static GUIStyle s_foldoutStyle;

        public static GUIStyle FoldoutStyle
        {
            get
            {
                if (s_foldoutStyle == null)
                {
                    s_foldoutStyle = new GUIStyle(EasyGUIStyles.Foldout)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_foldoutStyle.margin.top += 4;
                }
                return s_foldoutStyle;
            }
        }

         private static GUIStyle s_rightLabelStyle;

         public static GUIStyle RightLabelStyle
         {
             get
             {
                 if (s_rightLabelStyle == null)
                 {
                     s_rightLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                     {
                         alignment = TextAnchor.MiddleLeft,
                     };
                     s_rightLabelStyle.fontSize += 2;
                 }
                 return s_rightLabelStyle;
             }
         }

        private IExpressionEvaluator _labelEvaluator;
        [CanBeNull] private IExpressionEvaluator _tooltipEvaluator;
        [CanBeNull] private IExpressionEvaluator _rightLabelEvaluator;
        [CanBeNull] private IExpressionEvaluator _sideLineColorGetterEvaluator;
        [CanBeNull] private IExpressionEvaluator _rightLabelColorGetterEvaluator;
        [CanBeNull] private IExpressionEvaluator _iconTextureGetterEvaluator;

        protected override void Initialize()
        {
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

            _labelEvaluator = ExpressionEvaluatorFactory
                .Evaluate(Attribute.Label, targetType)
                .WithExpressionFlag()
                .Build();

            if (Attribute.Tooltip.IsNotNullOrWhiteSpace())
            {
                _tooltipEvaluator = ExpressionEvaluatorFactory
                    .Evaluate(Attribute.Tooltip, targetType)
                    .WithExpressionFlag()
                    .Build();
            }

            if (Attribute.RightLabel.IsNotNullOrWhiteSpace())
            {
                _rightLabelEvaluator = ExpressionEvaluatorFactory
                    .Evaluate(Attribute.RightLabel, targetType)
                    .WithExpressionFlag()
                    .Build();
            }

            if (Attribute.RightLabelColorGetter.IsNotNullOrWhiteSpace())
            {
                _rightLabelColorGetterEvaluator = ExpressionEvaluatorFactory
                    .Evaluate(Attribute.RightLabelColorGetter, targetType)
                    .Build();
            }

            if (Attribute.IconTextureGetter.IsNotNullOrWhiteSpace())
            {
                _iconTextureGetterEvaluator = ExpressionEvaluatorFactory
                    .Evaluate(Attribute.IconTextureGetter, targetType)
                    .Build();
            }

            if (Attribute.SideLineColorGetter.IsNotNullOrWhiteSpace())
            {
                _sideLineColorGetterEvaluator = ExpressionEvaluatorFactory
                    .Evaluate(Attribute.SideLineColorGetter, targetType)
                    .Build();
            }
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_tooltipEvaluator != null && _tooltipEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_rightLabelEvaluator != null && _rightLabelEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_rightLabelColorGetterEvaluator != null && _rightLabelColorGetterEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_iconTextureGetterEvaluator != null && _iconTextureGetterEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (_sideLineColorGetterEvaluator != null && _sideLineColorGetterEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label)
        {
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);

            EasyEditorGUI.BeginBox();

            GUILayout.Space(-3);
            EditorGUILayout.BeginHorizontal("Button", GUILayout.ExpandWidth(true), GUILayout.Height(30));

            Color sideLineColor = Color.green;
            if (_sideLineColorGetterEvaluator != null)
            {
                sideLineColor = _sideLineColorGetterEvaluator.Evaluate<Color>(resolveTarget);
            }

            EasyGUIHelper.PushColor(sideLineColor);
            GUILayout.Box(GUIContent.none, EasyGUIStyles.WhiteBoxStyle, GUILayout.Width(3), GUILayout.Height(30));
            EasyGUIHelper.PopColor();

            if (_iconTextureGetterEvaluator != null)
            {
                var iconTexture = _iconTextureGetterEvaluator.Evaluate<Texture>(resolveTarget);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            var labelText = _labelEvaluator.Evaluate<string>(resolveTarget);
            var tooltipText = _tooltipEvaluator?.Evaluate<string>(resolveTarget);

            var foldoutRect = EditorGUILayout.GetControlRect(true, 30, FoldoutStyle);
            Element.State.Expanded = EasyEditorGUI.Foldout(
                foldoutRect,
                Element.State.Expanded,
                EditorHelper.TempContent(labelText, tooltipText),
                FoldoutStyle);

            if (_rightLabelEvaluator != null)
            {
                var rightLabel = EditorHelper.TempContent(_rightLabelEvaluator.Evaluate<string>(resolveTarget));
                var rightLabelColor = Color.white;
                if (_rightLabelColorGetterEvaluator != null)
                {
                    rightLabelColor = _rightLabelColorGetterEvaluator.Evaluate<Color>(resolveTarget);
                }

                var rightLabelSize = RightLabelStyle.CalcSize(rightLabel);

                EasyGUIHelper.PushColor(rightLabelColor);
                GUI.Label(foldoutRect.AlignRight(rightLabelSize.x).SubY(2f), rightLabel, RightLabelStyle);
                EasyGUIHelper.PopColor();
            }

            EditorGUILayout.EndHorizontal();
        }

        protected override void EndDrawGroup()
        {
            EasyEditorGUI.EndBox();
        }
    }
}
