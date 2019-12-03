using System;
using System.Collections.Generic;
using DarkBestiary.Attributes;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AttributeData : Identity<int>
    {
        public int Index;
        public string NameKey;
        public string DescriptionKey;
        public string Icon;
        public bool IsPrimary;
        public AttributeType Type;
        public List<PropertyModifierData> PropertyModifiers = new List<PropertyModifierData>();
    }
}