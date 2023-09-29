using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemModifierData : Identity<int>
    {
        public bool IsSuffix;
        public string? SuffixTextKey;
        public List<ItemAttributeModifierData> Attributes = new();
        public List<ItemPropertyModifierData> Properties = new();
        public List<int> Categories = new();
    }

    [Serializable]
    public class ItemAttributeModifierData
    {
        public int AttributeId;
        public float Amount;
    }

    [Serializable]
    public class ItemPropertyModifierData
    {
        public int PropertyId;
        public float Amount;
    }
}