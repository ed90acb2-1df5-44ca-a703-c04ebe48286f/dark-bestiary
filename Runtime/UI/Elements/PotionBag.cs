using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class PotionBag : MonoBehaviour
    {
        [SerializeField] private InventoryItemSlot[] m_Slots;

        private InventoryComponent m_Inventory;

        public void Initialize(InventoryComponent inventory)
        {
            foreach (var slot in m_Slots)
            {
                slot.InventoryItem.Dropped += OnItemDropped;
                slot.InventoryItem.DoubleClicked += OnItemRightClicked;
                slot.InventoryItem.RightClicked += OnItemRightClicked;
            }

            m_Inventory = inventory;
            m_Inventory.ItemPicked += OnItemPicked;
            m_Inventory.ItemRemoved += OnItemRemoved;

            Refresh();
        }

        public void Terminate()
        {
            foreach (var slot in m_Slots)
            {
                slot.InventoryItem.Dropped -= OnItemDropped;
                slot.InventoryItem.DoubleClicked -= OnItemRightClicked;
                slot.InventoryItem.RightClicked -= OnItemRightClicked;
            }

            m_Inventory.ItemPicked -= OnItemPicked;
            m_Inventory.ItemRemoved -= OnItemRemoved;
        }

        private void OnItemDropped(ItemDroppedEventData data)
        {
            if (!m_Slots.Contains(data.InventorySlot))
            {
                return;
            }

            var previousSlot = m_Slots.First(s => s.InventoryItem == data.InventoryItem);
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

            m_Inventory.MaybeUse(item.Item);
        }

        private void Refresh()
        {
            foreach (var slot in m_Slots)
            {
                slot.ChangeItem(Item.CreateEmpty());
            }

            var consumables = m_Inventory.Items
                .Where(i => i.IsPotion)
                .OrderBy(i => i.Name);

            var index = 0;

            foreach (var consumable in consumables)
            {
                if (!m_Slots.IndexInBounds(index))
                {
                    break;
                }

                m_Slots[index].ChangeItem(consumable);
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