using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    /// <summary>
    /// 可拖拽的列表项 - 包装任意对象使其可被拖拽
    /// </summary>
    public static class DraggableListHelper
    {
        /// <summary>
        /// 开始拖拽操作
        /// </summary>
        public static void StartDrag<T>(T item, VisualElement sourceElement) where T : class
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.SetGenericData("DraggableListItem", item);
            DragAndDrop.StartDrag(item?.ToString() ?? "Item");
        }

        /// <summary>
        /// 为列表项元素添加拖拽支持
        /// </summary>
        public static void SetupItemDrag<T>(VisualElement itemElement, T itemData) where T : class
        {
            // 使用闭包捕获拖拽状态
            Vector2 startPos = Vector2.zero;
            float dragStartTime = 0f;
            bool hasStartedDrag = false;

            itemElement.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0 && evt.clickCount == 1)
                {
                    startPos = evt.mousePosition;
                    dragStartTime = (float)EditorApplication.timeSinceStartup;
                    hasStartedDrag = false;
                }
            });

            itemElement.RegisterCallback<MouseMoveEvent>(dragEvt =>
            {
                if (hasStartedDrag) return;
                // 只有在按住左键时才检测拖拽
                if (dragEvt.pressedButtons != 1) return;

                float dragDistance = Vector2.Distance(dragEvt.mousePosition, startPos);
                float dragDuration = (float)EditorApplication.timeSinceStartup - dragStartTime;

                // 移动超过5像素或持续超过0.2秒，开始拖拽
                if (dragDistance > 5 || dragDuration > 0.2f)
                {
                    hasStartedDrag = true;
                    StartDrag(itemData, itemElement);
                    dragEvt.StopPropagation();
                }
            });
        }
    }
}
