using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class InventoryPanel : MonoBehaviour
    {
        public event Payload<InventoryItem> ItemRightClicked;
        public event Payload<InventoryItem> ItemDoubleClicked;

        [SerializeField] private InventoryPanelFilter filter;
        [SerializeField] private InventoryItemSlot slotPrefab;
        [SerializeField] private Transform slotContainer;
        [SerializeField] private CurrencyView currencyPrefab;
        [SerializeField] private Transform currencyContainer;
        [SerializeField] private Interactable sortButton;

        public List<InventoryItemSlot> Slots { get; } = new List<InventoryItemSlot>();

        private readonly List<CurrencyView> currencies = new List<CurrencyView>();

        private InventoryComponent inventory;
        private Item deleting;

        public void Initialize(InventoryComponent inventory)
        {
            this.inventory = inventory;
            this.inventory.Sorted += OnSorted;
            this.inventory.Expanded += OnExpanded;
            this.inventory.ItemPicked += OnItemPicked;
            this.inventory.ItemRemoved += OnItemRemoved;
            this.inventory.ItemsSwapped += OnItemsSwapped;
            this.inventory.ItemStackCountChanged += OnItemStackCountChanged;

            foreach (var item in this.inventory.Items)
            {
                AddInventorySlot(item);
            }

            foreach (var currency in this.inventory.GetCurrencies())
            {
                var currencyView = Instantiate(this.currencyPrefab, this.currencyContainer);
                currencyView.Initialize(currency);
                this.currencies.Add(currencyView);
            }

            if (this.filter != null)
            {
                this.filter.Changed += OnFilterChanged;
            }

            this.sortButton.PointerUp += OnSortButtonPointerUp;
        }

        public void Terminate()
        {
            this.inventory.Sorted -= OnSorted;
            this.inventory.Expanded -= OnExpanded;
            this.inventory.ItemPicked -= OnItemPicked;
            this.inventory.ItemRemoved -= OnItemRemoved;
            this.inventory.ItemsSwapped -= OnItemsSwapped;
            this.inventory.ItemStackCountChanged -= OnItemStackCountChanged;

            foreach (var inventorySlot in Slots)
            {
                inventorySlot.InventoryItem.RightClicked -= OnItemRightClicked;
                inventorySlot.InventoryItem.Clicked -= OnItemClicked;
                inventorySlot.InventoryItem.EndDrag -= OnItemEndDrag;
                inventorySlot.ItemDroppedIn -= OnItemDroppedIn;
            }

            foreach (var currency in this.currencies)
            {
                currency.Terminate();
            }

            this.sortButton.PointerUp -= OnSortButtonPointerUp;
        }

        public void Block(Item item)
        {
            var slot = Slots.FirstOrDefault(s => s.InventoryItem.Item == item);

            if (slot == null)
            {
                return;
            }

            slot.InventoryItem.Block();
        }

        public void Unblock(Item item)
        {
            var slot = Slots.FirstOrDefault(s => s.InventoryItem.Item == item);

            if (slot == null)
            {
                return;
            }

            slot.InventoryItem.Unblock();
        }

        private void AddInventorySlot(Item item)
        {
            var inventorySlot = Instantiate(this.slotPrefab, this.slotContainer);
            inventorySlot.Construct(item);
            inventorySlot.InventoryItem.RightClicked += OnItemRightClicked;
            inventorySlot.InventoryItem.DoubleClicked += OnItemDoubleClicked;
            inventorySlot.InventoryItem.Clicked += OnItemClicked;
            inventorySlot.InventoryItem.EndDrag += OnItemEndDrag;
            inventorySlot.ItemDroppedIn += OnItemDroppedIn;

            Slots.Add(inventorySlot);
        }

        private void OnSortButtonPointerUp()
        {
            this.inventory.Sort();
        }

        private void OnFilterChanged()
        {
            var categories = this.filter.GetItemCategories();

            foreach (var inventorySlot in Slots)
            {
                var item = inventorySlot.InventoryItem.Item;

                if (item.IsEmpty)
                {
                    continue;
                }

                inventorySlot.InventoryItem.Unblock();

                if (categories.Count > 0 && !categories.Any(category => category.Contains(item.Type)))
                {
                    inventorySlot.InventoryItem.Block();
                }

                if (string.IsNullOrEmpty(this.filter.Search))
                {
                    continue;
                }

                if (!item.Name.LikeIgnoreCase($"%{this.filter.Search}%"))
                {
                    inventorySlot.InventoryItem.Block();
                }
            }
        }

        private void OnItemStackCountChanged(ItemStackCountChangedEventData data)
        {
            Slots[data.Index].InventoryItem.UpdateStackCount();
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            if (data.InventoryItem.Item.IsGem &&
                data.InventorySlot.InventoryItem.Item.IsSocketable &&
                data.InventorySlot.InventoryItem.Item.HasEmptySockets)
            {
                data.InventorySlot.InventoryItem.Item.InsertSocket(data.InventoryItem.Item);
                return;
            }

            if (!this.inventory.Contains(data.InventoryItem.Item) && data.InventoryItem.Item.Equipment == null)
            {
                this.inventory.Pickup(data.InventoryItem.Item.Clone(), data.InventorySlot.InventoryItem.Item);
                data.InventoryItem.Item.Inventory.Remove(data.InventoryItem.Item);
                return;
            }

            this.inventory.Swap(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
        }

        private void OnExpanded(InventoryComponent inventory)
        {
            for(var index = Slots.Count; index < this.inventory.Items.Count; index++)
            {
                AddInventorySlot(this.inventory.Items[index]);
            }
        }

        private void OnSorted(InventoryComponent inventory)
        {
            for (var i = 0; i < this.inventory.Items.Count; i++)
            {
                Slots[i].ChangeItem(this.inventory.Items[i]);
            }
        }

        private void OnItemRemoved(ItemRemovedEventData data)
        {
            Slots[data.Index].ChangeItem(data.Empty);
        }

        private void OnItemPicked(ItemPickupEventData data)
        {
            Slots[data.Index].ChangeItem(data.Item);
        }

        private void OnItemsSwapped(ItemsSwappedIndexEventData data)
        {
            Slots[data.Index1].ChangeItem(data.Item1);
            Slots[data.Index2].ChangeItem(data.Item2);
        }

        private void OnItemClicked(InventoryItem inventoryItem)
        {
            if (!Input.GetKey(KeyCode.LeftShift) || inventoryItem.Item.StackCount < 2)
            {
                return;
            }

            ItemSplitStackPanel.Instance.Show(inventoryItem.Item);
            ItemSplitStackPanel.Instance.SplitStackConfirmed += OnSplitStackConfirmed;
            ItemSplitStackPanel.Instance.SplitStackCancelled += OnSplitStackCancelled;
        }

        private void OnSplitStackConfirmed(Item item, int stack)
        {
            if (this.inventory.Contains(item))
            {
                this.inventory.PickupDoNotStack(item.Clone().SetStack(stack));
                this.inventory.Remove(item, stack);
            }

            OnSplitStackCancelled();
        }

        private void OnSplitStackCancelled()
        {
            ItemSplitStackPanel.Instance.SplitStackConfirmed -= OnSplitStackConfirmed;
            ItemSplitStackPanel.Instance.SplitStackCancelled -= OnSplitStackCancelled;
        }

        private void OnItemDoubleClicked(InventoryItem item)
        {
            ItemDoubleClicked?.Invoke(item);
        }

        private void OnItemRightClicked(InventoryItem inventoryItem)
        {
            ItemRightClicked?.Invoke(inventoryItem);
        }

        private void OnItemEndDrag(InventoryItem inventoryItem)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Delete(inventoryItem.Item);
            }
        }

        private void Delete(Item item)
        {
            this.deleting = item;

            ConfirmationWindow.Instance.Confirmed += OnItemDeletionConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnItemDeletionCancelled;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_item_delete_confirmation").ToString(this.deleting.ColoredName),
                I18N.Instance.Get("ui_delete")
            );
        }

        private void OnItemDeletionConfirmed()
        {
            this.inventory.Remove(this.deleting);

            OnItemDeletionCancelled();
        }

        private void OnItemDeletionCancelled()
        {
            this.deleting = null;

            ConfirmationWindow.Instance.Confirmed -= OnItemDeletionConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnItemDeletionCancelled;
        }
    }
}