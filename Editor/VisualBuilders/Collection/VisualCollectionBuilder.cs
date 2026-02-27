using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolkit.Serialization;
using UnityEngine.UIElements;

namespace EasyToolkit.Inspector.Editor
{
    public class VisualCollectionBuilder<TCollection, TItem> : VisualValueBuilder<TCollection>
        where TCollection : IList<TItem>
    {
        public new ICollectionElement Element => base.Element as ICollectionElement;

        public new ICollectionEntry<TCollection, TItem> ValueEntry =>
            base.ValueEntry as ICollectionEntry<TCollection, TItem>;

        public IOrderedCollectionEntry<TCollection, TItem> OrderedValueEntry =>
            ValueEntry as IOrderedCollectionEntry<TCollection, TItem>;

        private DraggableListView _draggableListView;

        protected override bool CanBuildElement(IValueElement element)
        {
            return element.Definition.Roles.IsCollection();
        }

        protected override void Initialize()
        {
            ValueEntry.AfterCollectionChanged += OnCollectionChanged;
        }

        protected override VisualElement CreateVisualElement()
        {
            var listView = new DraggableListView(
                Element.Label.text,
                new List<TItem>(ValueEntry.SmartValue),
                ValueEntry.ItemType,
                MakeDraggableItem,
                BindDraggableItem);

            listView.ItemsIndexChanged += OnDraggableListItemIndexChanged;
            listView.SelectionChanged += OnDraggableListSelectionChanged;
            listView.AddNewItemRequested += OnAddDraggableItem;
            listView.RemoveItemRequested += OnRemoveDraggableItem;

            _draggableListView = listView;
            return listView;
        }

        private VisualElement MakeDraggableItem()
        {
            var root = new VisualElement();
            root.AddToClassList("draggable-list-item");
            // var content = new VisualElement();
            // content.AddToClassList("list-item-content");
            // root.Add(content);
            return root;
        }

        private void BindDraggableItem(VisualElement element, int index)
        {
            var item = Element.LogicalChildren[index];
            item.SpecificOwningVisualElement = element;
            item.Draw(forceDraw: true);
        }

        private void OnDraggableListItemIndexChanged(int oldIndex, int newIndex)
        {
            var value = OrderedValueEntry.GetItemAt(0, oldIndex);
            OrderedValueEntry.RemoveItemAt(0, oldIndex);
            OrderedValueEntry.InsertItemAt(0, newIndex, value);
        }

        private void OnDraggableListSelectionChanged(object selectedItem)
        {
        }

        private void OnAddDraggableItem(DraggableListView listView)
        {
            var value = EasySerializer.DeserializeFromBinary<TItem>(Array.Empty<byte>());
            ValueEntry.AddItem(0, value);
        }

        private void OnCollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            var changeType = e.ChangeType;
            var item = e.Item;
            var itemIndex = e.ItemIndex ?? -1;

            Element.SharedContext.Tree.QueueCallback(() =>
            {
                switch (changeType)
                {
                    case CollectionChangeType.Add:
                        _draggableListView.AddItem(item);
                        break;
                    case CollectionChangeType.Remove:
                        _draggableListView.RemoveItem(item);
                        break;
                    case CollectionChangeType.Insert:
                        _draggableListView.InsertItem(itemIndex, item);
                        break;
                    case CollectionChangeType.RemoveAt:
                        _draggableListView.RemoveItemAt(itemIndex);
                        break;
                    case CollectionChangeType.Clear:
                        _draggableListView.ClearItems();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        private void OnRemoveDraggableItem(DraggableListView listView, int obj)
        {
            OrderedValueEntry.RemoveItemAt(0, obj);
        }

        protected override void Dispose()
        {
            ValueEntry.AfterCollectionChanged -= OnCollectionChanged;
        }
    }
}
