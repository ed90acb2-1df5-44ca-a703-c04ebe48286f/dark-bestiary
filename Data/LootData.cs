using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class LootData : Identity<int>
    {
        public string Name;
        public int Count;
        public LootItemData[] Items;
    }
}