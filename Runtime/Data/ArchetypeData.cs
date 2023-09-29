using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ArchetypeData : Identity<int>
    {
        public List<ArchetypeAttributeModifierData> Attributes = new();
        public List<ArchetypePropertyModifierData> Properties = new();
    }

    [Serializable]
    public class ArchetypeAttributeModifierData
    {
        public int AttributeId;
        public float Min;
        public float Max;
        public CurveType CurveType;
    }

    [Serializable]
    public class ArchetypePropertyModifierData
    {
        public int PropertyId;
        public float Min;
        public float Max;
        public CurveType CurveType;
    }
}