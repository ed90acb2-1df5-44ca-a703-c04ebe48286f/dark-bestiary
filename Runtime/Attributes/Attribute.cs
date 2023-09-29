using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Modifiers;
using UnityEngine;

namespace DarkBestiary.Attributes
{
    public class Attribute
    {
        public event Action<Attribute> Changed;

        public int Id { get; }
        public int Index { get; }
        public AttributeType Type { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public bool IsPrimary { get; }
        public string Icon { get; }
        public float Base { get; private set; }
        public int Points { get; private set; }
        public PropertiesComponent Properties { get; private set; }

        private readonly List<PropertyModifier> m_PropertyModifiers = new();

        private float m_ArchetypeBonus;
        private List<AttributeModifier> m_Modifiers = new();

        public Attribute(AttributeData data, IPropertyRepository propertyRepository)
        {
            Id = data.Id;
            Index = data.Index;
            Type = data.Type;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Icon = data.Icon;
            IsPrimary = data.IsPrimary;

            foreach (var propertyModifierData in data.PropertyModifiers)
            {
                var modifier = new PropertyModifier(
                    propertyRepository.Find(propertyModifierData.PropertyId),
                    propertyModifierData
                );

                m_PropertyModifiers.Add(modifier);
            }
        }

        public void Initialize(PropertiesComponent properties)
        {
            Properties = properties;
            Properties.ApplyModifiers(m_PropertyModifiers);

            OnChange();
        }

        public void AddPoint(int amount = 1)
        {
            Points += amount;
            OnChange();
        }

        public void SubtractPoint(int amount = 1)
        {
            Points -= amount;
            OnChange();
        }

        public void ResetPoints()
        {
            Points = 0;
            OnChange();
        }

        public void SetArchetypeBonus(float archetypeBonus)
        {
            m_ArchetypeBonus = archetypeBonus;
            OnChange();
        }

        public bool HasModifiers()
        {
            return m_Modifiers.Count > 0;
        }

        public bool HasNegativeModifiers()
        {
            return m_Modifiers.Any(modifier => modifier.GetAmount() < 0);
        }

        public float Value()
        {
            var flat = Base + m_ArchetypeBonus + Points;
            flat = m_Modifiers.Where(m => m.Type == ModifierType.Flat).Aggregate(flat, (current, modifier) => modifier.Modify(current));

            var aggregated = flat;

            foreach (var modifier in m_Modifiers.Where(m => m.Type != ModifierType.Flat).OrderBy(m => m.GetAmount()))
            {
                aggregated += modifier.Modify(flat) - flat;
            }

            return Mathf.Ceil(aggregated);
        }

        public Attribute Increase(float amount)
        {
            Base += amount;
            OnChange();
            return this;
        }

        public Attribute Decrease(float amount)
        {
            Base -= amount;
            OnChange();
            return this;
        }

        public void AddModifier(AttributeModifier modifier)
        {
            modifier.StackChanged += OnModifierStackChanged;
            m_Modifiers.Add(modifier);
            m_Modifiers = m_Modifiers.OrderBy(m => m.Type).ThenBy(m => m.Data.AttributeFraction != null).ToList();
            OnChange();
        }

        public void RemoveModifier(AttributeModifier modifier)
        {
            modifier.StackChanged -= OnModifierStackChanged;
            m_Modifiers.Remove(modifier);
            OnChange();
        }

        private void OnModifierStackChanged(FloatModifier modifier)
        {
            OnChange();
        }

        private void OnChange()
        {
            Changed?.Invoke(this);

            foreach (var propertyModifier in m_PropertyModifiers)
            {
                propertyModifier.ChangeStack(propertyModifier.StackCount);
            }
        }
    }
}