using System.Linq;
using DarkBestiary.Components;

namespace DarkBestiary.Items
{
    public class TwoHandedWeaponEquipmentStrategy : DefaultEquipmentStrategy
    {
        public override void Prepare(Item item, EquipmentSlot slot, EquipmentComponent equipment)
        {
            base.Prepare(item, slot, equipment);

            foreach (var s in equipment.Slots.Where(s => s.Item.IsWeapon))
            {
                if (s.IsEmpty)
                {
                    continue;
                }

                equipment.Unequip(s.Item);
            }
        }
    }
}