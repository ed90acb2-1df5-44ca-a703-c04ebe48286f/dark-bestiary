using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Items;
using DarkBestiary.Modifiers;
using DarkBestiary.Properties;
using UnityEngine;

namespace DarkBestiary.Masteries
{
    public abstract class Mastery
    {
        private const int c_MaxLevel = 6;

        public static event Action<Mastery> AnyMasteryLevelUp;

        public int Id { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public Experience Experience { get; private set; }
        public GameObject Owner { get; private set; }

        protected readonly MasteryData Data;

        private readonly ItemModifier m_Modifier;

        private PropertiesComponent m_Properties;
        private AttributesComponent m_Attributes;

        private readonly List<PropertyModifier> m_PropertyModifiers = new();
        private readonly List<AttributeModifier> m_AttributeModifiers = new();

        protected Mastery(MasteryData data, ItemModifier modifier)
        {
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);

            m_Modifier = modifier;
            Data = data;
        }

        public void Construct(int level, int experience)
        {
            Experience = new Experience(level, c_MaxLevel, experience, RequiredExperienceAtLevel);
        }

        public void Initialize(GameObject owner)
        {
            Owner = owner;

            m_Properties = Owner.GetComponent<PropertiesComponent>();
            m_Attributes = Owner.GetComponent<AttributesComponent>();

            OnInitialize();

            Experience.LevelUp += OnLevelUp;
            OnLevelUp(Experience);
        }

        public void Terminate()
        {
            Experience.LevelUp -= OnLevelUp;

            OnTerminate();
        }

        public List<AttributeModifier> GetAttributeModifiers(int level)
        {
            return m_Modifier.GetAttributeModifiers(level);
        }

        public List<PropertyModifier> GetPropertyModifiers(int level)
        {
            return m_Modifier.GetPropertyModifiers(level);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnTerminate()
        {
        }

        private void OnLevelUp(Experience experience)
        {
            RefreshModifiers();

            AnyMasteryLevelUp?.Invoke(this);
        }

        protected virtual void RefreshModifiers()
        {
            RemoveModifiers();
            ApplyModifiers();
        }

        protected void ApplyModifiers(int times = 1)
        {
            RemoveModifiers();

            var level = Experience.Level - 1;

            if (level <= 0)
            {
                return;
            }

            for (var i = 0; i < times; i++)
            {
                m_AttributeModifiers.AddRange(GetAttributeModifiers(level));
                m_PropertyModifiers.AddRange(GetPropertyModifiers(level));
            }

            m_Attributes.ApplyModifiers(m_AttributeModifiers);
            m_Properties.ApplyModifiers(m_PropertyModifiers);
        }

        protected void RemoveModifiers()
        {
            m_Attributes.RemoveModifiers(m_AttributeModifiers);
            m_AttributeModifiers.Clear();

            m_Properties.RemoveModifiers(m_PropertyModifiers);
            m_PropertyModifiers.Clear();
        }

        private static int RequiredExperienceAtLevel(int level)
        {
            if (level < 2)
            {
                return 0;
            }

            return (int) (29 + Math.Pow(level + 2, 5) / 2);
        }

        public string GetDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.DamageIncrease));
        }

        public string GetMinionDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.MinionDamage));
        }

        public string GetMinionHealthString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.MinionHealth));
        }

        public string GetCriticalChanceString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.CriticalHitChance));
        }

        public string GetCriticalDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.CriticalHitDamage));
        }

        public string GetArmorPenetrationString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.ArmorPenetration));
        }

        public string GetMagicPenetrationString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.MagicPenetration));
        }

        public string GetBlockAmountString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.BlockAmount));
        }

        public string GetDodgeChanceString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.Dodge));
        }

        public string GetHealingString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.HealingIncrease));
        }

        public string GetArcaneDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.ArcaneDamageIncrease));
        }

        public string GetColdDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.ColdDamageIncrease));
        }

        public string GetFireDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.FireDamageIncrease));
        }

        public string GetHolyDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.HolyDamageIncrease));
        }

        public string GetLightningDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.LightningDamageIncrease));
        }

        public string GetPoisonDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.PoisonDamageIncrease));
        }

        public string GetShadowDamageString(GameObject entity)
        {
            return PropertyModifierValueString(PropertyModifiersForTooltip().First(m => m.Property.Type == PropertyType.ShadowDamageIncrease));
        }

        private List<PropertyModifier> PropertyModifiersForTooltip()
        {
            // Note: At level '0' I need to display values for level '1'.
            var level = Mathf.Max(1, Experience.Level - 1);

            return GetPropertyModifiers(level);
        }

        private string PropertyModifierValueString(PropertyModifier modifier, bool absolute = false)
        {
            modifier.Entity = modifier.Entity ?? Game.Instance.Character.Entity;

            var value = modifier.Modify(0);
            value = absolute ? Mathf.Abs(value) : value;

            return Property.ValueString(modifier.Property.Type, value);
        }
    }
}