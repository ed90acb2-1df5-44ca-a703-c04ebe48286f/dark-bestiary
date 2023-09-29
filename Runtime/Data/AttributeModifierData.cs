using System;
using DarkBestiary.Attributes;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AttributeModifierData
    {
        public float Amount;
        public int AttributeId;
        public ModifierType Type;

        public float MaxAttributeFraction;
        public AttributeFraction AttributeFraction = new();

        public AttributeModifierData()
        {
        }

        public AttributeModifierData(int attributeId, float amount, ModifierType type)
        {
            Amount = amount;
            AttributeId = attributeId;
            Type = type;
        }
    }

    [Serializable]
    public class AttributeFraction
    {
        public AttributeType AttributeType = AttributeType.None;
        public float Fraction;
    }
}