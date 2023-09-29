using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemCategoryData : Identity<int>
    {
        public string NameKey;
        public ItemCategoryType Type;
        public List<int> ItemTypes = new();
    }
}