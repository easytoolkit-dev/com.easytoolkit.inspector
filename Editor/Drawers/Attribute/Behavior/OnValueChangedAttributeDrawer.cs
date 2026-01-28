using System;
using System.Reflection;
using EasyToolkit.Core.Textual;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using EasyToolkit.Inspector.Attributes;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    public class OnValueChangedAttributeDrawer<T> : EasyAttributeDrawer<OnValueChangedAttribute, T>
    {
        private MethodInfo _methodInfo;
        private string _error;

        protected override void Draw(GUIContent label)
        {
            if (_error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(_error, MessageType.Error);
                return;
            }

            if (_methodInfo == null)
            {
                var targetType = ElementUtility.GetOwnerTypeWithAttribute(Element, Attribute);
                try
                {
                    try
                    {
                        _methodInfo = targetType.GetOverloadMethod(Attribute.Method, MemberAccessFlags.All, ValueEntry.ValueType);
                    }
                    catch (Exception e)
                    {
                        _methodInfo = targetType.GetOverloadMethod(Attribute.Method, MemberAccessFlags.All);
                    }

                    ValueEntry.AfterValueChanged += OnValueChanged;
                }
                catch (Exception e)
                {
                    _error = e.Message;
                }
            }

            CallNextDrawer(label);
        }

        private void OnValueChanged(object sender, ValueChangedEventArgs eventArgs)
        {
            var value = ValueEntry.GetWeakValue(eventArgs.TargetIndex);
            var args = _methodInfo.GetParameters().Length == 0 ? null : new object[] { value };
            if (_methodInfo.IsStatic)
            {
                _methodInfo.Invoke(null, args);
            }
            else
            {
                var target = ElementUtility.GetOwnerWithAttribute(Element, Attribute, eventArgs.TargetIndex);
                _methodInfo.Invoke(target, args);
            }
        }
    }
}
