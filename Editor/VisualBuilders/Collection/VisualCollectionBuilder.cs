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

        public override VisualElement CreateVisualElement()
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
            return root;
        }

        private void BindDraggableItem(VisualElement element, int index)
        {
            element.Clear();
            var item = Element.LogicalChildren[index];
            item.SpecificOwningVisualElement = element;
            item.Draw();
        }

        private void OnDraggableListItemIndexChanged(int oldIndex, int newIndex)
        {
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
            switch (e.ChangeType)
            {
                case CollectionChangeType.Add:
                    _draggableListView.AddItem(e.Item);
                    break;
                case CollectionChangeType.Remove:
                    _draggableListView.RemoveItem(e.Item);
                    break;
                case CollectionChangeType.Insert:
                    _draggableListView.InsertItem(e.ItemIndex.Value, e.Item);
                    break;
                case CollectionChangeType.RemoveAt:
                    _draggableListView.RemoveItemAt(e.ItemIndex.Value);
                    break;
                case CollectionChangeType.Clear:
                    _draggableListView.ClearItems();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnRemoveDraggableItem(DraggableListView listView, int obj)
        {
            OrderedValueEntry.RemoveItemAt(0, obj);
        }

        public override void Dispose()
        {
            ValueEntry.AfterCollectionChanged -= OnCollectionChanged;
        }
    }
}
