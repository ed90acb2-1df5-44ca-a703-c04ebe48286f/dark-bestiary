using System;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemTypeData : Identity<int>
    {
        public string NameKey;
        public ItemTypeType Type;
        public int MaxSocketCount;
        public int MasteryId;
        public EquipmentStrategyType EquipmentStrategyType;
    }
}