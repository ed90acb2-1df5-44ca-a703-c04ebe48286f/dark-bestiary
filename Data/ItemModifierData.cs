using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ItemModifierData : Identity<int>
    {
        public bool IsSuffix;
        public string SuffixTextKey;
        public int BehaviourId;
        public int RarityId;
        public float Weight;
        public List<AttributeModifierCurveData> Attributes = new List<AttributeModifierCurveData>();
        public List<PropertyModifierCurveData> Properties = new List<PropertyModifierCurveData>();
        public List<int> ItemModifiers = new List<int>();
        public List<int> Categories = new List<int>();
    }
}