using System;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemTypeMapper : Mapper<ItemTypeData, ItemType>
    {
        public override ItemTypeData ToData(ItemType entity)
        {
            throw new NotImplementedException();
        }

        public override ItemType ToEntity(ItemTypeData data)
        {
            IEquipmentStrategy equipmentStrategy;

            switch (data.EquipmentStrategyType)
            {
                case EquipmentStrategyType.Default:
                    equipmentStrategy = new DefaultEquipmentStrategy();
                    break;
                case EquipmentStrategyType.TwoHandedWeapon:
                    equipmentStrategy = new TwoHandedWeaponEquipmentStrategy();
                    break;
                case EquipmentStrategyType.OneHandedWeapon:
                    equipmentStrategy = new OneHandedWeaponEquipmentStrategy();
                    break;
                case EquipmentStrategyType.NotEquippable:
                    equipmentStrategy = new NotEquippableEquipmentStrategy();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var itemType = new ItemType(data, equipmentStrategy);

            return itemType;
        }
    }
}