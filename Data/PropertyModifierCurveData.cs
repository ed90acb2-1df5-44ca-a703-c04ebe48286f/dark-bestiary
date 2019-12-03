using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class PropertyModifierCurveData
    {
        public int PropertyId;
        public float Min;
        public float Max;
        public CurveType CurveType;
    }
}