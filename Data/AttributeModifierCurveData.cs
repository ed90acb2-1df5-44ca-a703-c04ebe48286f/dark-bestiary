using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AttributeModifierCurveData
    {
        public int AttributeId;
        public float Min;
        public float Max;
        public CurveType CurveType;
    }
}