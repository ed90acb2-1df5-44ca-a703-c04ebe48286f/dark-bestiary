using System.Linq;
using DarkBestiary.Components;

namespace DarkBestiary.Items
{
    public class OneHandedWeaponEquipmentStrategy : DefaultEquipmentStrategy
    {
        public override void Prepare(Item item, EquipmentSlot slot, EquipmentComponent equipment)
        {
            base.Prepare(item, slot, equipment);

            foreach (var s in equipment.Slots.Where(s => s.Item.IsTwoHandedWeapon))
            {
                equipment.Unequip(s.Item);
            }
        }
    }
}