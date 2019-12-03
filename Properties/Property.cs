using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;
using DarkBestiary.Modifiers;
using UnityEngine;

namespace DarkBestiary.Properties
{
    public class Property
    {
        public event Payload<Property> Changed;

        public int Id { get; }
        public int Index { get; }
        public bool IsUnscalable { get; }
        public I18NString Name { get; }
        public PropertyType Type { get; }
        public float Base { get; private set; }
        public List<PropertyModifier> Modifiers { get; private set; } = new List<PropertyModifier>();

        private readonly PropertyData data;

        private AttributesComponent attributes;
        private float archetypeBonus;

        public Property(PropertyData data)
        {
            Id = data.Id;
            Index = data.Index;
            Type = data.Type;
            IsUnscalable = data.IsUnscalable;
            Name = I18N.Instance.Get(data.NameKey);
            this.data = data;
        }

        public void SetArchetypeBonus(float archetypeBonus)
        {
            this.archetypeBonus = archetypeBonus;
            OnChange();
        }

        public float Value()
        {
            var flat = Mathf.Max(Base, this.data.Min) + this.data.Start + this.archetypeBonus;

            flat = Modifiers
                .Where(m => m.Type == ModifierType.Flat)
                .ToList()
                .Aggregate(flat, (current, modifier) => modifier.Modify(current));

            var aggregated = flat;

            foreach (var modifier in Modifiers.Where(m => m.Type != ModifierType.Flat).OrderBy(m => m.GetAmount()))
            {
                aggregated += modifier.Modify(flat) - flat;
            }

            return Mathf.Clamp(aggregated, this.data.Min, this.data.Max);
        }

        public string ValueString()
        {
            return ValueString(Type, Value());
        }

        public static bool IsFractional(PropertyType type)
        {
            return !new[]
            {
                PropertyType.AttackPower,
                PropertyType.SpellPower,
                PropertyType.Health,
                PropertyType.HealthRegeneration,
                PropertyType.Thorns,
                PropertyType.RangedWeaponExtraRange,
                PropertyType.MaximumActionPoints,
                PropertyType.Alchemy
            }.Contains(type);
        }

        public bool IsFractional()
        {
            return IsFractional(Type);
        }

        public static string ValueString(PropertyType type, float amount)
        {
            if (IsFractional(type))
            {
                return (amount * 100).ToString("0.00") + "%";
            }

            return ((int) Mathf.Floor(amount)).ToString();
        }

        public Property Increase(float amount)
        {
            Base += amount;
            OnChange();
            return this;
        }

        public Property Decrease(float amount)
        {
            Base -= amount;
            OnChange();
            return this;
        }

        public bool HasModifiers()
        {
            return Modifiers.Count > 0;
        }

        public bool HasNegativeModifiers()
        {
            return Modifiers.Any(modifier => modifier.GetAmount() < 0);
        }

        public void AddModifier(PropertyModifier modifier)
        {
            modifier.StackChanged += OnModifierStackChanged;
            Modifiers.Add(modifier);
            Modifiers = Modifiers.OrderBy(m => m.Type).ToList();
            OnChange();
        }

        public void RemoveModifier(PropertyModifier modifier)
        {
            modifier.StackChanged -= OnModifierStackChanged;
            Modifiers.Remove(modifier);
            OnChange();
        }

        private void OnModifierStackChanged(FloatModifier modifier)
        {
            OnChange();
        }

        public void OnChange()
        {
            Changed?.Invoke(this);
        }
    }
}