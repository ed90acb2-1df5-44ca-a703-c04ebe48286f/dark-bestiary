using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemTypeData : Identity<int>
    {
        public string NameKey;
        public ItemTypeType Type;
        public int MaxSocketCount;
        public int MaxRuneCount;
        public int MasteryId;
        public EquipmentStrategyType EquipmentStrategyType;
        public List<int> Categories = new();
    }
}