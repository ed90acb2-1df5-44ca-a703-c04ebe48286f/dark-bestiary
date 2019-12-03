using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ArchetypeData : Identity<int>
    {
        public List<AttributeModifierCurveData> Attributes = new List<AttributeModifierCurveData>();
        public List<PropertyModifierCurveData> Properties = new List<PropertyModifierCurveData>();
    }
}