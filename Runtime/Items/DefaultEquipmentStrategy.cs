using System.Linq;
using DarkBestiary.Components;

namespace DarkBestiary.Items
{
    public class DefaultEquipmentStrategy : IEquipmentStrategy
    {
        public virtual void Prepare(Item item, EquipmentSlot slot, EquipmentComponent equipment)
        {
            if (!item.IsUniqueEquipped)
            {
                return;
            }

            var sameEquippedItem = equipment.Slots.FirstOrDefault(s => s.Item.Id == item.Id)?.Item;

            if (sameEquippedItem == null)
            {
                return;
            }

            equipment.Unequip(sameEquippedItem);
        }
    }
}