using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class PotionBag : MonoBehaviour
    {
        [SerializeField] private InventoryItemSlot[] slots;

        private InventoryComponent inventory;

        public void Initialize(InventoryComponent inventory)
        {
            foreach (var slot in this.slots)
            {
                slot.InventoryItem.Dropped += OnItemDropped;
                slot.InventoryItem.DoubleClicked += OnItemRightClicked;
                slot.InventoryItem.RightClicked += OnItemRightClicked;
            }

            this.inventory = inventory;
            this.inventory.ItemPicked += OnItemPicked;
            this.inventory.ItemRemoved += OnItemRemoved;

            Refresh();
        }

        public void Terminate()
        {
            foreach (var slot in this.slots)
            {
                slot.InventoryItem.Dropped -= OnItemDropped;
                slot.InventoryItem.DoubleClicked -= OnItemRightClicked;
                slot.InventoryItem.RightClicked -= OnItemRightClicked;
            }

            this.inventory.ItemPicked -= OnItemPicked;
            this.inventory.ItemRemoved -= OnItemRemoved;
        }

        private void OnItemDropped(ItemDroppedEventData data)
        {
            if (!this.slots.Contains(data.InventorySlot))
            {
                return;
            }

            var previousSlot = this.slots.First(s => s.InventoryItem == data.InventoryItem);
            var currentSlotItem = data.InventorySlot.InventoryItem.Item;
            data.InventorySlot.ChangeItem(previousSlot.InventoryItem.Item);
            previousSlot.ChangeItem(currentSlotItem);
        }

        private void OnItemRightClicked(InventoryItem item)
        {
            if (item.Item.IsEmpty)
            {
                return;
            }

            this.inventory.MaybeUse(item.Item);
        }

        private void Refresh()
        {
            foreach (var slot in this.slots)
            {
                slot.ChangeItem(Item.CreateEmpty());
            }

            var consumables = this.inventory.Items
                .Where(i => i.IsPotion)
                .OrderBy(i => i.Name);

            var index = 0;

            foreach (var consumable in consumables)
            {
                if (!this.slots.IndexInBounds(index))
                {
                    break;
                }

                this.slots[index].ChangeItem(consumable);
                index++;
            }
        }

        private void OnItemPicked(ItemPickupEventData data)
        {
            Refresh();
        }

        private void OnItemRemoved(ItemRemovedEventData data)
        {
            Refresh();
        }
    }
}