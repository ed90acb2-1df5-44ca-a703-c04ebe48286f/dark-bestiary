using System;
using DarkBestiary.Modifiers;
using DarkBestiary.Properties;

namespace DarkBestiary.Data
{
    [Serializable]
    public class PropertyModifierData
    {
        public float Amount;
        public float MaxAttributeFraction;
        public string Formula;
        public int PropertyId;
        public ModifierType Type;
        public PropertyFraction PropertyFraction = new PropertyFraction();
        public AttributeFraction AttributeFraction = new AttributeFraction();

        public PropertyModifierData()
        {
        }

        public PropertyModifierData(float amount, ModifierType type)
        {
            this.Amount = amount;
            this.Type = type;
        }
    }

    [Serializable]
    public class PropertyFraction
    {
        public PropertyType PropertyType = PropertyType.None;
        public float Fraction;
    }
}