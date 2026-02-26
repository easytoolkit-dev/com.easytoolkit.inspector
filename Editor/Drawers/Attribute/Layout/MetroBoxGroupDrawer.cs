using EasyToolkit.Core.Textual;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using EasyToolkit.Inspector.Editor.Internal;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class MetroBoxGroupDrawer : EasyGroupDrawer<MetroBoxGroupAttribute>
    {
        private static GUIStyle s_boxHeaderLabelStyle;
        public static GUIStyle BoxHeaderLabelStyle
        {
            get
            {
                if (s_boxHeaderLabelStyle == null)
                {
                    s_boxHeaderLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_boxHeaderLabelStyle.margin.top += 4;
                }
                return s_boxHeaderLabelStyle;
            }
        }

        public static readonly GUIStyle BoxContainerStyle = new GUIStyle("TextArea")
        {
        };

        public static readonly Color HeaderBoxBackgroundColor = EasyGUIStyles.HeaderBoxBackgroundColor * 0.9f;

        private IExpressionEvaluator _labelEvaluator;
        private IExpressionEvaluator _iconTextureGetterEvaluator;

        protected override void Initialize()
        {
            var targetType = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerTypeWithAttribute(Element.AssociatedElement, Attribute);

            _labelEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                Attribute.Label, targetType, requireExpressionFlag: true);
            _iconTextureGetterEvaluator = ExpressionEvaluatorFactory.CreateEvaluator(
                Attribute.IconTextureGetter, targetType);
        }

        protected override void Draw(GUIContent label)
        {
            if (_labelEvaluator.TryGetError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty() && _iconTextureGetterEvaluator.TryGetError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.Draw(label);
        }

        protected override void BeginDrawGroup(GUIContent label)
        {
            Texture iconTexture = null;
            var resolveTarget = Element.AssociatedElement == null
                ? null
                : ElementUtility.GetOwnerWithAttribute(Element.AssociatedElement, Attribute);

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty())
            {
                iconTexture = _iconTextureGetterEvaluator.Evaluate<Texture>(resolveTarget);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            var labelText = _labelEvaluator.Evaluate<string>(resolveTarget);

            BeginDraw(EditorHelper.TempContent(labelText), iconTexture);
        }

        protected override void EndDrawGroup()
        {
            EndDraw();
        }

        public static void BeginDraw(GUIContent label, Texture iconTexture)
        {
            EasyEditorGUI.BeginIndentedVertical(BoxContainerStyle);

            GUILayout.Space(-3);
            var headerBgRect = EditorGUILayout.BeginHorizontal(EasyGUIStyles.BoxHeaderStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));

            if (Event.current.type == EventType.Repaint)
            {
                headerBgRect.x -= 3;
                headerBgRect.width += 6;
                EasyGUIHelper.PushColor(HeaderBoxBackgroundColor);
                GUI.DrawTexture(headerBgRect, Texture2D.whiteTexture);
                EasyGUIHelper.PopColor();
                EasyEditorGUI.DrawBorders(headerBgRect, 0, 0, 0, 1, EasyGUIStyles.BorderColor);
            }

            if (iconTexture != null)
            {
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            GUILayout.Label(label, BoxHeaderLabelStyle, GUILayout.Height(30));

            EditorGUILayout.EndHorizontal();
        }

        public static void EndDraw()
        {
            EasyEditorGUI.EndIndentedVertical();
        }
    }
}
