using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class InventoryPanel : MonoBehaviour
    {
        public event Payload<InventoryItem> ItemControlClicked;
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
        private Item enchantSource;
        private Item enchantTarget;

        public void ChangeInventory(InventoryComponent inventory)
        {
            UnsubscribeFromInventoryUpdates(this.inventory);

            this.inventory = inventory;

            SubscribeToInventoryUpdates(this.inventory);

            for (var i = 0; i < Slots.Count; i++)
            {
                Slots[i].ChangeItem(this.inventory.Items[i]);
            }

            OnFilterChanged();
        }

        public void Initialize(InventoryComponent inventory)
        {
            this.inventory = inventory;
            SubscribeToInventoryUpdates(this.inventory);

            Slots.AddRange(this.slotContainer.GetComponentsInChildren<InventoryItemSlot>());

            for (var i = 0; i < this.inventory.Items.Count; i++)
            {
                if (!Slots.IndexInBounds(i))
                {
                    Slots.Add(Instantiate(this.slotPrefab, this.slotContainer));
                }

                SetupInventorySlot(Slots[i], this.inventory.Items[i]);
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

            this.sortButton.PointerClick += OnSortButtonPointerClick;
        }

        public void Terminate()
        {
            UnsubscribeFromInventoryUpdates(this.inventory);

            foreach (var inventorySlot in Slots)
            {
                inventorySlot.InventoryItem.ControlClicked -= OnItemControlClicked;
                inventorySlot.InventoryItem.RightClicked -= OnItemRightClicked;
                inventorySlot.InventoryItem.Clicked -= OnItemClicked;
                inventorySlot.InventoryItem.EndDrag -= OnItemEndDrag;
                inventorySlot.ItemDroppedIn -= OnItemDroppedIn;
            }

            foreach (var currency in this.currencies)
            {
                currency.Terminate();
            }

            this.sortButton.PointerClick -= OnSortButtonPointerClick;
        }

        private void SubscribeToInventoryUpdates(InventoryComponent inventory)
        {
            inventory.Sorted += OnSorted;
            inventory.Expanded += OnExpanded;
            inventory.ItemPicked += OnItemPicked;
            inventory.ItemRemoved += OnItemRemoved;
            inventory.ItemsSwapped += OnItemsSwapped;
            inventory.ItemStackCountChanged += OnItemStackCountChanged;
        }

        private void UnsubscribeFromInventoryUpdates(InventoryComponent inventory)
        {
            inventory.Sorted -= OnSorted;
            inventory.Expanded -= OnExpanded;
            inventory.ItemPicked -= OnItemPicked;
            inventory.ItemRemoved -= OnItemRemoved;
            inventory.ItemsSwapped -= OnItemsSwapped;
            inventory.ItemStackCountChanged -= OnItemStackCountChanged;
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

        private void SetupInventorySlot(InventoryItemSlot slot, Item item)
        {
            slot.transform.SetParent(this.slotContainer);
            slot.InventoryItem.RightClicked += OnItemRightClicked;
            slot.InventoryItem.ControlClicked += OnItemControlClicked;
            slot.InventoryItem.DoubleClicked += OnItemDoubleClicked;
            slot.InventoryItem.Clicked += OnItemClicked;
            slot.InventoryItem.EndDrag += OnItemEndDrag;
            slot.ItemDroppedIn += OnItemDroppedIn;
            slot.ChangeItem(item);
        }

        private void OnSortButtonPointerClick()
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
            try
            {
                if (data.InventoryItem.Item.IsGem &&
                    data.InventorySlot.InventoryItem.Item.IsSocketable &&
                    data.InventorySlot.InventoryItem.Item.HasEmptySockets)
                {
                    data.InventorySlot.InventoryItem.Item.InsertSocket(data.InventoryItem.Item);
                    return;
                }

                if (data.InventoryItem.Item.IsEnchantment &&
                    data.InventorySlot.InventoryItem.Item.IsEnchantable)
                {
                    this.enchantSource = data.InventoryItem.Item;
                    this.enchantTarget = data.InventorySlot.InventoryItem.Item;

                    ConfirmationWindow.Instance.Cancelled += OnEnchantCancelled;
                    ConfirmationWindow.Instance.Confirmed += OnEnchantConfirmed;
                    ConfirmationWindow.Instance.Show(
                        I18N.Instance.Get("ui_confirm_enchant_x").ToString(data.InventorySlot.InventoryItem.Item.ColoredName),
                        I18N.Instance.Get("ui_confirm")
                    );

                    return;
                }

                if (!this.inventory.Contains(data.InventoryItem.Item) && data.InventoryItem.Item.Equipment == null)
                {
                    var item = data.InventoryItem.Item;
                    item.Inventory.Remove(item);
                    this.inventory.Pickup(item.Clone(), data.InventorySlot.InventoryItem.Item);
                    return;
                }

                this.inventory.Swap(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnEnchantConfirmed()
        {
            try
            {
                this.enchantTarget.Enchant(this.enchantSource);
                (this.enchantSource.Inventory ? this.enchantSource.Inventory : this.inventory)
                    .Remove(this.enchantSource, 1);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
            finally
            {
                OnEnchantCancelled();
            }
        }

        private void OnEnchantCancelled()
        {
            ConfirmationWindow.Instance.Cancelled -= OnEnchantCancelled;
            ConfirmationWindow.Instance.Confirmed -= OnEnchantConfirmed;

            this.enchantSource = null;
            this.enchantTarget = null;
        }

        private void OnExpanded(InventoryComponent inventory)
        {
            for (var index = Slots.Count; index < this.inventory.Items.Count; index++)
            {
                Slots.Add(Instantiate(this.slotPrefab, this.slotContainer));
                SetupInventorySlot(Slots[index], this.inventory.Items[index]);
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

        private void OnItemControlClicked(InventoryItem inventoryItem)
        {
            ItemControlClicked?.Invoke(inventoryItem);
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