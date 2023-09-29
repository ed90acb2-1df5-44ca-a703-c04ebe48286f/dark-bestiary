using System;
using DarkBestiary.Modifiers;
using DarkBestiary.Properties;

namespace DarkBestiary.Data
{
    [Serializable]
    public class PropertyModifierData
    {
        public int PropertyId;
        public float Amount;
        public ModifierType Type;

        public string? Formula;

        public float MaxAttributeFraction;
        public PropertyFraction PropertyFraction = new();
        public AttributeFraction AttributeFraction = new();

        public PropertyModifierData()
        {
        }

        public PropertyModifierData(int propertyId, float amount, ModifierType type)
        {
            PropertyId = propertyId;
            Amount = amount;
            Type = type;
        }
    }

    [Serializable]
    public class PropertyFraction
    {
        public PropertyType PropertyType = PropertyType.None;
        public float Fraction;
    }
}