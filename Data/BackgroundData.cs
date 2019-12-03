using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class BackgroundData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public int Gold;
        public List<int> Skills = new List<int>();
        public List<BackgroundItemData> Items = new List<BackgroundItemData>();
    }

    [Serializable]
    public class BackgroundItemData
    {
        public int ItemId;
        public int Count;
        public bool IsEquipped;
    }
}