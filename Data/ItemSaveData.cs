using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemSaveData
    {
        public int ItemId;
        public int StackCount;
        public int ForgeLevel;
        public int SharpeningLevel;
        public int SuffixId;
        public int RarityId;
        public int EnchantId;
        public bool IsMarkedAsIllusory;
        public List<int> Sockets = new List<int>();
        public List<int> Affixes = new List<int>();
        public List<int> Runes = new List<int>();
    }
}