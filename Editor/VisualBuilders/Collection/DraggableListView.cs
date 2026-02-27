using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public class DraggableListView : VisualElement
    {
        public readonly float DefaultHeight = 80 * 4;

        private ListView _listView;
        private Label _countLabel;
        private IList _items;
        private string _title;
        private Func<VisualElement> _makeItem;
        private Action<VisualElement, int> _bindItem;
        private Type _itemType;

        public IList Items => _items;

        public event Action<IEnumerable<int>> ItemsDropAdded;
        public event Action<int, int> ItemsIndexChanged;

        public event Action<object> SelectionChanged;

        public DraggableListView(string title, IList items, Type itemType, Func<VisualElement> makeItem,
            Action<VisualElement, int> bindItem)
        {
            _title = title;
            _items = items;
            _itemType = itemType;
            _makeItem = makeItem;
            _bindItem = bindItem;

            InitializeUI();
        }

        private void InitializeUI()
        {
            AddToClassList("draggable-list-container");

            // 创建标题栏
            VisualElement header = CreateHeader();
            Add(header);

            // 创建列表视图
            _listView = new ListView
            {
                itemsSource = _items,
                name = "draggable-list-view",
                showBorder = true,
                showFoldoutHeader = false,
                selectionType = SelectionType.Single,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                style = { height = DefaultHeight }
            };

            // 设置高度计算方式
            _listView.makeItem = MakeItemWithRemoveButton;
            _listView.bindItem = BindItemWithRemoveButton;

            _listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            _listView.fixedItemHeight = 0;

            // 注册重新排序回调
            _listView.itemIndexChanged += OnItemsIndexChanged;
            _listView.selectedIndicesChanged += OnSelectionChanged;

            // 启用拖拽
            SetupDragAndDrop();

            Add(_listView);
        }

        private VisualElement CreateHeader()
        {
            VisualElement header = new VisualElement { name = "draggable-list-header" };
            header.AddToClassList("draggable-list-header");

            // 标题
            Label titleLabel = new Label(_title) { name = "list-title-label" };
            titleLabel.AddToClassList("list-title-label");

            // 数量显示
            _countLabel = new Label($"{_items.Count} 项") { name = "list-count-label" };
            _countLabel.AddToClassList("list-count-label");

            // 添加/删除按钮
            Button addButton = new Button(OnAddClicked)
            {
                name = "list-add-button",
                text = "+"
            };
            addButton.AddToClassList("list-icon-button");

            Button removeButton = new Button(OnRemoveClicked)
            {
                name = "list-remove-button",
                text = "-"
            };
            removeButton.AddToClassList("list-icon-button");

            header.Add(titleLabel);
            header.Add(_countLabel);
            header.Add(addButton);
            header.Add(removeButton);

            return header;
        }

        private void SetupDragAndDrop()
        {
            _listView.RegisterCallback<DragEnterEvent>(OnDragEnter);
            _listView.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            _listView.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            _listView.RegisterCallback<DragPerformEvent>(OnDragPerform);
        }

        private void OnDragEnter(DragEnterEvent evt)
        {
            if (CanAcceptDrag(evt))
            {
                _listView.AddToClassList("drag-over");
            }
        }

        private void OnDragLeave(DragLeaveEvent evt)
        {
            _listView.RemoveFromClassList("drag-over");
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (CanAcceptDrag(evt))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                evt.StopPropagation();
            }
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            if (!CanAcceptDrag(evt)) return;

            DragAndDrop.AcceptDrag();

            // 获取拖拽的数据并添加到列表
            object draggedData = DragAndDrop.GetGenericData("DraggableListItem");
            if (draggedData.GetType().IsDerivedFrom(_itemType) && !_items.Contains(draggedData))
            {
                int insertIndex = _listView.selectedIndex >= 0 ? _listView.selectedIndex : _items.Count;
                _items.Insert(insertIndex, draggedData);
                _listView.RefreshItems();
                UpdateCountLabel();
                ItemsDropAdded?.Invoke(new[] { insertIndex });
            }

            _listView.RemoveFromClassList("drag-over");
            evt.StopPropagation();
        }

        private bool CanAcceptDrag(EventBase evt)
        {
            return DragAndDrop.GetGenericData("DraggableListItem").GetType().IsDerivedFrom(_itemType);
        }

        private void OnItemsIndexChanged(int oldIndex, int newIndex)
        {
            ItemsIndexChanged?.Invoke(oldIndex, newIndex);
        }

        private void OnSelectionChanged(IEnumerable<int> indices)
        {
            foreach (int index in indices)
            {
                if (index >= 0 && index < _items.Count)
                {
                    SelectionChanged?.Invoke(_items[index]);
                    break;
                }
            }
        }

        private void OnAddClicked()
        {
            // 触发添加事件，由外部提供新项
            AddNewItemRequested?.Invoke(this);
        }

        private void OnRemoveClicked()
        {
            if (_listView.selectedIndex >= 0 && _listView.selectedIndex < _items.Count)
            {
                RemoveItemRequested?.Invoke(this, _listView.selectedIndex);
            }
        }

        private void UpdateCountLabel()
        {
            _countLabel.text = $"{_items.Count} 项";
        }

        /// <summary>
        /// 请求添加新项时触发
        /// </summary>
        public event Action<DraggableListView> AddNewItemRequested;

        /// <summary>
        /// 请求删除项时触发
        /// </summary>
        public event Action<DraggableListView, int> RemoveItemRequested;

        /// <summary>
        /// 刷新列表显示
        /// </summary>
        public void Refresh()
        {
            _listView.RefreshItems();
            UpdateCountLabel();
        }

        /// <summary>
        /// 添加新项到列表
        /// </summary>
        public void AddItem(object item)
        {
            _items.Add(item);
            Refresh();
        }

        /// <summary>
        /// 在指定索引插入项
        /// </summary>
        public void InsertItem(int index, object item)
        {
            _items.Insert(index, item);
            Refresh();
        }

        /// <summary>
        /// 移除指定索引的项
        /// </summary>
        public bool RemoveItemAt(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _items.RemoveAt(index);
                Refresh();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除指定项
        /// </summary>
        public bool RemoveItem(object item)
        {
            int index = _items.IndexOf(item);
            return RemoveItemAt(index);
        }

        public void ClearItems()
        {
            _items.Clear();
            Refresh();
        }

        /// <summary>
        /// 获取当前选中项
        /// </summary>
        public bool TryGetSelectedItem(out object value)
        {
            if (_listView.selectedIndex >= 0 && _listView.selectedIndex < _items.Count)
            {
                value = _items[_listView.selectedIndex];
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 设置选中项
        /// </summary>
        public void SetSelectedIndex(int index)
        {
            if (index >= 0 && index < _items.Count)
            {
                _listView.selectedIndex = index;
            }
        }

        /// <summary>
        /// 创建带有删除按钮的列表项
        /// </summary>
        private VisualElement MakeItemWithRemoveButton()
        {
            VisualElement itemContainer = new VisualElement
            {
                name = "list-item-container",
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    paddingRight = 4
                }
            };

            VisualElement originalItem = _makeItem();
            originalItem.style.flexGrow = 1;
            itemContainer.Add(originalItem);

            Button removeButton = CreateIconButton(EasyEditorIcons.X.HighlightedTexture, 14);
            removeButton.name = "item-remove-button";
            removeButton.AddToClassList("item-remove-button");
            removeButton.style.flexShrink = 0;
            itemContainer.Add(removeButton);

            return itemContainer;
        }

        /// <summary>
        /// 绑定带有删除按钮的列表项
        /// </summary>
        private void BindItemWithRemoveButton(VisualElement element, int index)
        {
            VisualElement originalItem = element[0];
            if (element[1] is not Button removeButton) return;

            _bindItem(originalItem, index);

            removeButton.clickable = null;
            removeButton.RegisterCallback<ClickEvent>(evt =>
            {
                RemoveItemRequested?.Invoke(this, index);
                evt.StopPropagation();
            });
        }

        /// <summary>
        /// 创建图标按钮
        /// </summary>
        private Button CreateIconButton(Texture2D icon, int iconSize)
        {
            Button button = new Button
            {
                style =
                {
                    width = iconSize + 6,
                    height = iconSize + 6,
                    paddingLeft = 2,
                    paddingRight = 2,
                    paddingTop = 2,
                    paddingBottom = 2,
                    backgroundImage = icon,
                    // backgroundPosition = BackgroundPosition.Center,
                    // backgroundRepeat = BackgroundRepeat.NoRepeat,
                    backgroundSize = new BackgroundSize(iconSize, iconSize)
                }
            };
            return button;
        }
    }
}
