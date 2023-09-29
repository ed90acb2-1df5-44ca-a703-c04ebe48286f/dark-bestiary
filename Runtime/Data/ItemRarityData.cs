using System;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemRarityData : Identity<int>
    {
        public string NameKey;
        public RarityType Type;
        public string ColorCode;
    }
}