using System;
using DarkBestiary.Properties;

namespace DarkBestiary.Data
{
    [Serializable]
    public class PropertyData : Identity<int>
    {
        public int Index;
        public string NameKey;
        public PropertyType Type;
        public bool IsUnscalable;
        public float Min;
        public float Max;
        public float Start;
    }
}