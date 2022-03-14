using System;
using DarkBestiary.Attributes;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AttributeModifierData
    {
        public float Amount;
        public float MaxAttributeFraction;
        public int AttributeId;
        public ModifierType Type;
        public AttributeFraction AttributeFraction = new AttributeFraction();

        public AttributeModifierData()
        {
        }

        public AttributeModifierData(int attributeId, float amount, ModifierType type)
        {
            this.Amount = amount;
            this.AttributeId = attributeId;
            this.Type = type;
        }
    }

    [Serializable]
    public class AttributeFraction
    {
        public AttributeType AttributeType = AttributeType.None;
        public float Fraction;
    }
}