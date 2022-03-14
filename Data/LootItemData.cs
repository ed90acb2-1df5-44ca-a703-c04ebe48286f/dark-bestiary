using System;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class LootItemData : Identity<int>
    {
        public LootItemType Type;
        public int ItemId;
        public int TableId;
        public int RarityId;
        public int CategoryId;
        public float Probability;
        public int StackMax;
        public int StackMin;
        public bool Enabled;
        public bool Unique;
        public bool Guaranteed;
        public bool IgnoreLevel;
    }
}