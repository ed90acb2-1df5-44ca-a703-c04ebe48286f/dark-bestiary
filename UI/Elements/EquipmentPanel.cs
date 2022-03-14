using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class EquipmentPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform container;

        [Header("Weapon swap")]
        [SerializeField] private Interactable swapWeaponButton;

        private List<InventoryItemSlot> slots;
        private EquipmentComponent equipment;
        private InventoryComponent inventory;
        private Item enchantSource;
        private Item enchantTarget;

        public void Initialize(EquipmentComponent equipment)
        {
            this.inventory = equipment.GetComponent<InventoryComponent>();

            this.equipment = equipment;
            this.equipment.ItemEquipped += OnItemEquipped;
            this.equipment.ItemUnequipped += OnItemUnequipped;
            this.equipment.ItemsSwapped += OnItemsSwapped;

            this.slots = this.container.GetComponentsInChildren<InventoryItemSlot>().ToList();

            for (var i = 0; i < equipment.Slots.Count; i++)
            {
                this.slots[i].ChangeItem(equipment.Slots[i].Item);
                this.slots[i].ItemDroppedIn += OnItemDroppedIn;
                this.slots[i].ItemDroppedOut += OnItemDroppedOut;
                this.slots[i].InventoryItem.RightClicked += OnItemRightClicked;
            }

            this.swapWeaponButton.PointerClick += OnSwapWeaponButtonPointerClick;

            InventoryItem.AnyBeginDrag += OnAnyItemBeginDrag;
            InventoryItem.AnyEndDrag += OnAnyItemEndDrag;
        }

        public void Terminate()
        {
            this.equipment.ItemEquipped -= OnItemEquipped;
            this.equipment.ItemUnequipped -= OnItemUnequipped;
            this.equipment.ItemsSwapped -= OnItemsSwapped;

            this.swapWeaponButton.PointerClick -= OnSwapWeaponButtonPointerClick;

            foreach (var slot in this.slots)
            {
                slot.ItemDroppedIn -= OnItemDroppedIn;
                slot.ItemDroppedOut -= OnItemDroppedOut;
                slot.InventoryItem.RightClicked -= OnItemRightClicked;
            }

            InventoryItem.AnyBeginDrag -= OnAnyItemBeginDrag;
            InventoryItem.AnyEndDrag -= OnAnyItemEndDrag;
        }

        public void Refresh()
        {
            for (var i = 0; i < this.equipment.Slots.Count; i++)
            {
                this.slots[i].ChangeItem(this.equipment.Slots[i].Item);
            }
        }

        private void OnSwapWeaponButtonPointerClick()
        {
            try
            {
                this.equipment.SwapWeapon();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnAnyItemBeginDrag(InventoryItem item)
        {
            var suitable = this.equipment.FindSuitableSlot(item.Item);

            if (suitable == null)
            {
                return;
            }

            this.slots[this.equipment.Slots.IndexOf(suitable)].Highlight();
        }

        private void OnAnyItemEndDrag(InventoryItem item)
        {
            foreach (var slot in this.slots)
            {
                slot.Unhighlight();
            }
        }

        private void OnItemsSwapped()
        {
            Refresh();
        }

        private void OnItemUnequipped(Item item)
        {
            Refresh();
        }

        private void OnItemEquipped(Item item)
        {
            Refresh();
        }

        private void OnItemDroppedIn(ItemDroppedEventData data)
        {
            try
            {
                if (this.equipment.IsEquipped(data.InventoryItem.Item))
                {
                    this.equipment.Swap(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
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

                if (data.InventoryItem.Item.IsGem &&
                    data.InventorySlot.InventoryItem.Item.IsSocketable &&
                    data.InventorySlot.InventoryItem.Item.HasEmptySockets)
                {
                    data.InventorySlot.InventoryItem.Item.InsertSocket(data.InventoryItem.Item);
                    return;
                }

                this.equipment.EquipIntoSlot(data.InventoryItem.Item, this.equipment.Slots[this.slots.IndexOf(data.InventorySlot)]);
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

        private void OnItemDroppedOut(ItemDroppedEventData data)
        {
            if (this.slots.Any(slot => slot.Equals(data.InventorySlot)))
            {
                return;
            }

            Timer.Instance.WaitForEndOfFrame(() =>
            {
                try
                {
                    this.equipment.Unequip(data.InventoryItem.Item, data.InventorySlot.InventoryItem.Item);
                }
                catch (GameplayException exception)
                {
                    UiErrorFrame.Instance.ShowMessage(exception.Message);
                }
            });
        }

        private void OnItemRightClicked(InventoryItem inventoryItem)
        {
            try
            {
                this.equipment.Unequip(inventoryItem.Item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}