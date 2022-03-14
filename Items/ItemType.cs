using System.Collections.Generic;
using DarkBestiary.Data;

namespace DarkBestiary.Items
{
    public class ItemType
    {
        public int Id { get; }
        public I18NString Name { get; }
        public ItemTypeType Type { get; }
        public int MasteryId { get; }
        public IEquipmentStrategy EquipmentStrategy { get; }
        public int MaxSocketCount { get; }
        public int MaxRuneCount { get; }
        public List<int> Categories { get; } = new List<int>();

        public ItemType(ItemTypeData data, IEquipmentStrategy equipmentStrategy)
        {
            Id = data.Id;
            MasteryId = data.MasteryId;
            Name = I18N.Instance.Get(data.NameKey);
            Type = data.Type;
            MaxSocketCount = data.MaxSocketCount;
            MaxRuneCount = data.MaxRuneCount;
            EquipmentStrategy = equipmentStrategy;
            Categories = data.Categories;
        }
    }
}