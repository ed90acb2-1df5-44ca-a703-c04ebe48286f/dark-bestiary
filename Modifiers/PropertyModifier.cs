using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Properties;
using UnityEngine;

namespace DarkBestiary.Modifiers
{
    public class PropertyModifier : FloatModifier
    {
        public GameObject Entity { get; set; }

        public Property Property { get; }
        public PropertyModifierData Data { get; }

        public PropertyModifier(Property property, PropertyModifierData data) : base(data.Amount, data.Type)
        {
            Property = property;
            Data = data;
        }

        public override string ToString()
        {
            return $"+{Property.ValueString(Property.Type, Property.Value())} {Property.Name}";
        }

        public override float Modify(float value)
        {
            var property = Entity.GetComponent<PropertiesComponent>().Get(Data.PropertyId);
            var contains = property.Modifiers.Contains(this);

            if (contains)
            {
                property.Modifiers.Remove(this);
            }

            var result = PerformModification(value);

            if (contains)
            {
                property.Modifiers.Add(this);
            }

            return result;
        }

        private float PerformModification(float value)
        {
            if (!string.IsNullOrEmpty(Data.Formula))
            {
                return value + Formula.Evaluate(Data.Formula, Entity, Entity);
            }

            value = base.Modify(value);

            if (Data.PropertyFraction.PropertyType != PropertyType.None)
            {
                value += Entity.GetComponent<PropertiesComponent>().Get(Data.PropertyFraction.PropertyType).Value() * (Data.PropertyFraction.Fraction * StackCount);
            }

            var attributes = Entity.GetComponent<AttributesComponent>();

            if (Data.MaxAttributeFraction > 0)
            {
                value += attributes.Attributes.Values.Where(a => a.IsPrimary).Max(a => a.Value()) * (Data.MaxAttributeFraction * StackCount);
            }

            if (Data.AttributeFraction.AttributeType != AttributeType.None)
            {
                value += attributes.Get(Data.AttributeFraction.AttributeType).Value() * (Data.AttributeFraction.Fraction * StackCount);
            }

            return value;
        }
    }
}