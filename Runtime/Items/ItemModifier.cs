using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Modifiers;

namespace DarkBestiary.Items
{
    public class ItemModifier
    {
        public int Id { get; }
        public I18NString SuffixText { get; }
        public List<AttributeModifier> AttributeModifiers { get; }
        public List<PropertyModifier> PropertyModifiers { get; }

        public ItemModifier(int id, I18NString suffixText, List<AttributeModifier> attributeModifiers, List<PropertyModifier> propertyModifiers)
        {
            Id = id;
            SuffixText = suffixText;
            AttributeModifiers = attributeModifiers;
            PropertyModifiers = propertyModifiers;
        }

        public List<AttributeModifier> GetAttributeModifiers(float multiplier)
        {
            return AttributeModifiers.Select(ScaleModifier).ToList();

            AttributeModifier ScaleModifier(AttributeModifier modifier)
            {
                var data = new AttributeModifierData(modifier.Data.AttributeId, modifier.Data.Amount * multiplier, modifier.Data.Type);
                return new AttributeModifier(modifier.Attribute, data);
            }
        }

        public List<PropertyModifier> GetPropertyModifiers(float multiplier)
        {
            return PropertyModifiers.Select(ScaleModifier).ToList();

            PropertyModifier ScaleModifier(PropertyModifier modifier)
            {
                var data = new PropertyModifierData(modifier.Data.PropertyId, modifier.Data.Amount * multiplier, modifier.Data.Type);
                return new PropertyModifier(modifier.Property, data);
            }
        }
    }
}