using DarkBestiary.Components;

namespace DarkBestiary.Items
{
    public interface IEquipmentStrategy
    {
        void Prepare(Item item, EquipmentSlot slot, EquipmentComponent equipment);
    }
}