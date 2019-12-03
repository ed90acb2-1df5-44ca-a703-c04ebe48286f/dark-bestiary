using System;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class MasteryData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public int ModifierId;
        public string Type;
        public DamageType DamageType;
        public ItemTypeType ItemType;
    }
}