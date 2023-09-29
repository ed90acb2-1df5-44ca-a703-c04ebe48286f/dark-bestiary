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
        public int StackMax;
        public int StackMin;

        public float Probability;
        public bool Enabled;
        public bool Unique;
        public bool Guaranteed;
    }
}