using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class InventoryPanel : MonoBehaviour
    {
        public event Action<InventoryItem> ItemControlClicked;
        public event Action<InventoryItem> ItemRightClicked;
        public event Action<InventoryItem> ItemDoubleClicked;

        [SerializeField] private InventoryPanelFilter m_Filter;
        [SerializeField] private InventoryItemSlot m_SlotPrefab;
        [SerializeField] private Transform m_SlotContainer;
        [SerializeField] private CurrencyView m_CurrencyPrefab;
        [SerializeField] private Transform m_CurrencyContainer;
        [SerializeField] private Interactable m_SortButton;

        public List<InventoryItemSlot> Slots { get; } = new();

        private readonly List<CurrencyView> m_Currencies = new();

        private InventoryComponent m_Inventory;
        private Item m_Deleting;
        private Item m_EnchantSource;
        private Item m_EnchantTarget;

        public void ChangeInventory(InventoryComponent inventory)
        {
            UnsubscribeFromInventoryUpdates(m_Inventory);

            m_Inventory = inventory;

            SubscribeToInventoryUpdates(m_Inventory);

            for (var i = 0; i < Slots.Count; i++)
            {
                Slots[i].ChangeItem(m_Inventory.Items[i]);
            }

            OnFilterChanged();
        }

        public void Initialize(InventoryComponent inventory)
        {
            m_Inventory = inventory;
            SubscribeToInventoryUpdates(m_Inventory);

            Slots.AddRange(m_SlotContainer.GetComponentsInChildren<InventoryItemSlot>());

            for (var i = 0; i < m_Inventory.Items.Count; i++)
            {
                if (!Slots.IndexInBounds(i))
                {
                    Slots.Add(Instantiate(m_SlotPrefab, m_SlotContainer));
                }

                SetupInventorySlot(Slots[i], m_Inventory.Items[i]);
            }

            foreach (var currency in m_Inventory.GetCurrencies())
            {
                var currencyView = Instantiate(m_CurrencyPrefab, m_CurrencyContainer);
                currencyView.Initialize(currency);
                m_Currencies.Add(currencyView);
            }

            if (m_Filter != null)
            {
                m_Filter.Changed += OnFilterChanged;
            }

            m_SortButton.PointerClick += OnSortButtonPointerClick;
        }

        public void Terminate()
        {
            UnsubscribeFromInventoryUpdates(m_Inventory);

            foreach (var inventorySlot in Slots)
            {
                inventorySlot.InventoryItem.ControlClicked -= OnItemControlClicked;
                inventorySlot.InventoryItem.RightClicked -= OnItemRightClicked;
                inventorySlot.InventoryItem.Clicked -= OnItemClicked;
                inventorySlot.InventoryItem.EndDrag -= OnItemEndDrag;
                inventorySlot.ItemDroppedIn -= OnItemDroppedIn;
            }

            foreach (var currency in m_Currencies)
            {
                currency.Terminate();
            }

            m_SortButton.PointerClick -= OnSortButtonPointerClick;
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
            slot.transform.SetParent(m_SlotContainer);
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
            m_Inventory.Sort();
        }

        private void OnFilterChanged()
        {
            var categories = m_Filter.GetItemCategories();

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

                if (string.IsNullOrEmpty(m_Filter.Search))
                {
                    continue;
                }

                if (!item.Name.Contains($"%{m_Filter.Search}%"))
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
                    m_EnchantSource = data.InventoryItem.Item;
                    m_EnchantTarget = data.InventorySlot.InventoryItem.Item;

                    ConfirmationWindow.Instance.Cancelled += OnEnchantCancelled;
                    ConfirmationWindow.Instance.Confirmed += OnEnchantConfirmed;
                    ConfirmationWindow.Instance.Show(
                        I18N.Instance.Get("ui_confirm_enchant_x").ToString(data.InventorySlot.InventoryItem.Item.ColoredName),
                        I18N.Instance.Get("ui_confirm")
                    );

                    return;
                }

                if (!m_Inventory.Contains(data.InventoryItem.Item) && data.InventoryItem.Item.Equipment == null)
                {
                    var item = data.InventoryItem.Item;
                    item.Inventory.Remove(item);
                    m_Inventory.Pickup(item.Clone(), data.InventorySlot.InventoryItem.Item);
                    return;
                }

                m_Inventory.Swap(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
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
                m_EnchantTarget.Enchant(m_EnchantSource);
                (m_EnchantSource.Inventory ? m_EnchantSource.Inventory : m_Inventory)
                    .Remove(m_EnchantSource, 1);
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

            m_EnchantSource = null;
            m_EnchantTarget = null;
        }

        private void OnExpanded(InventoryComponent inventory)
        {
            for (var index = Slots.Count; index < m_Inventory.Items.Count; index++)
            {
                Slots.Add(Instantiate(m_SlotPrefab, m_SlotContainer));
                SetupInventorySlot(Slots[index], m_Inventory.Items[index]);
            }
        }

        private void OnSorted(InventoryComponent inventory)
        {
            for (var i = 0; i < m_Inventory.Items.Count; i++)
            {
                Slots[i].ChangeItem(m_Inventory.Items[i]);
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
            if (m_Inventory.Contains(item))
            {
                m_Inventory.PickupDoNotStack(item.Clone().SetStack(stack));
                m_Inventory.Remove(item, stack);
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
            m_Deleting = item;

            ConfirmationWindow.Instance.Confirmed += OnItemDeletionConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnItemDeletionCancelled;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_item_delete_confirmation").ToString(m_Deleting.ColoredName),
                I18N.Instance.Get("ui_delete")
            );
        }

        private void OnItemDeletionConfirmed()
        {
            m_Inventory.Remove(m_Deleting);

            OnItemDeletionCancelled();
        }

        private void OnItemDeletionCancelled()
        {
            m_Deleting = null;

            ConfirmationWindow.Instance.Confirmed -= OnItemDeletionConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnItemDeletionCancelled;
        }
    }
}