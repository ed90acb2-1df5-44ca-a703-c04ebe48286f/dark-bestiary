using System;
using UnityEngine;

namespace DarkBestiary.Items
{
    public class EquipmentSlot
    {
        public string Label { get; }
        public EquipmentSlotType Type { get; }
        public Item Item { get; private set; }
        public bool IsEmpty => Item.IsEmpty;

        private GameObject attachment;

        public EquipmentSlot(string label, EquipmentSlotType type)
        {
            Label = label;
            Type = type;
            Item = Item.CreateEmpty();
        }

        public bool CanEquip(Item item)
        {
            if (item.SlotType == EquipmentSlotType.AnyHand)
            {
                return Type == EquipmentSlotType.MainHand || Type == EquipmentSlotType.OffHand;
            }

            if (item.IsTwoHandedWeapon)
            {
                return Type == EquipmentSlotType.MainHand;
            }

            return item.SlotType == Type;
        }

        public void Put(Item item)
        {
            if (!item.IsEmpty && !CanEquip(item))
            {
                throw new Exception($"Wrong slot type {Type} for type of item {item.Type}");
            }

            Item = item;
        }

        public void Clear()
        {
            if (IsEmpty)
            {
                return;
            }

            Item = Item.Inventory.CreateEmptyItem();
        }
    }
}