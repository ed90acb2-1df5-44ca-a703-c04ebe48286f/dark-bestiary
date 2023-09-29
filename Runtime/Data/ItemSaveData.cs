using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemSaveData
    {
        public int ItemId;
        public int StackCount;
        public int SuffixId;
        public int RarityId;
        public int EnchantId;
        public List<int> Sockets = new();
        public List<int> Affixes = new();
        public List<int> Runes = new();
    }
}