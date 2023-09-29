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
        public List<int> Skills = new();
        public List<BackgroundItemData> Items = new();
    }

    [Serializable]
    public class BackgroundItemData
    {
        public int ItemId;
        public int Count;
        public bool IsEquipped;
    }
}