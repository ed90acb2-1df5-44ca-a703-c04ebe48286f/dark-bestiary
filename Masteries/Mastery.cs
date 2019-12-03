using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Modifiers;
using DarkBestiary.Properties;
using UnityEngine;

namespace DarkBestiary.Masteries
{
    public abstract class Mastery
    {
        public const int MaxLevel = 5;

        public static event Payload<Mastery> AnyMasteryLevelUp;

        public int Id { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public Experience Experience { get; private set; }
        public GameObject Owner { get; private set; }

        protected readonly MasteryData Data;

        private readonly ItemModifier modifier;

        private PropertiesComponent properties;
        private AttributesComponent attributes;

        private List<PropertyModifier> propertyModifiers;
        private List<AttributeModifier> attributeModifiers;

        protected Mastery(MasteryData data, ItemModifier modifier)
        {
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);

            this.modifier = modifier;
            this.Data = data;
        }

        public void Construct(int level, int experience)
        {
            Experience = new Experience(level, MaxLevel, experience, RequiredExperienceAtLevel);
        }

        public void Initialize(GameObject owner)
        {
            Owner = owner;

            this.properties = Owner.GetComponent<PropertiesComponent>();
            this.attributes = Owner.GetComponent<AttributesComponent>();

            OnInitialize();

            Experience.LevelUp += OnLevelUp;
            OnLevelUp(Experience);
        }

        public void Terminate()
        {
            Experience.LevelUp -= OnLevelUp;

            OnTerminate();
        }

        public List<AttributeModifier> GetAttributeModifiers()
        {
            return this.modifier.GetAttributeModifiers(Experience.Level, 0);
        }

        public List<PropertyModifier> GetPropertyModifiers()
        {
            return this.modifier.GetPropertyModifiers(Experience.Level, 0);
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
            if (this.attributeModifiers != null || this.propertyModifiers != null)
            {
                RemoveModifiers();
            }

            this.attributeModifiers = new List<AttributeModifier>();
            this.propertyModifiers = new List<PropertyModifier>();

            for (var i = 0; i < times; i++)
            {
                this.attributeModifiers.AddRange(GetAttributeModifiers());
                this.propertyModifiers.AddRange(GetPropertyModifiers());
            }

            this.attributes.ApplyModifiers(this.attributeModifiers);
            this.properties.ApplyModifiers(this.propertyModifiers);
        }

        protected void RemoveModifiers()
        {
            if (this.attributeModifiers != null)
            {
                this.attributes.RemoveModifiers(this.attributeModifiers);
            }

            if (this.propertyModifiers != null)
            {
                this.properties.RemoveModifiers(this.propertyModifiers);
            }

            this.attributeModifiers = null;
            this.propertyModifiers = null;
        }

        private static int RequiredExperienceAtLevel(int level)
        {
            if (level < 2)
            {
                return 0;
            }

            return (int) (29 + Math.Pow(level + 2, 5) / 2);
        }

        public string GetMinionDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.MinionDamage));
        }

        public string GetMinionHealthString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.MinionHealth));
        }

        public string GetCriticalChanceString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.CriticalHitChance));
        }

        public string GetCriticalDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.CriticalHitDamage));
        }

        public string GetArmorPenetrationString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.ArmorPenetration));
        }

        public string GetMagicPenetrationString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.MagicPenetration));
        }

        public string GetBlockAmountString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.BlockAmount));
        }

        public string GetDodgeChanceString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.Dodge));
        }

        public string GetHealingString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.HealingIncrease));
        }

        public string GetArcaneDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.ArcaneDamageIncrease));
        }

        public string GetColdDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.ColdDamageIncrease));
        }

        public string GetFireDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.FireDamageIncrease));
        }

        public string GetHolyDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.HolyDamageIncrease));
        }

        public string GetLightningDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.LightningDamageIncrease));
        }

        public string GetPoisonDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.PoisonDamageIncrease));
        }

        public string GetShadowDamageString(GameObject entity)
        {
            return PropertyModifierValueString(GetPropertyModifiers().First(m => m.Property.Type == PropertyType.ShadowDamageIncrease));
        }

        private string PropertyModifierValueString(PropertyModifier modifier, bool absolute = false)
        {
            modifier.Entity = modifier.Entity ?? CharacterManager.Instance.Character.Entity;

            var value = modifier.Modify(0);
            value = absolute ? Mathf.Abs(value) : value;

            return Property.ValueString(modifier.Property.Type, value);
        }
    }
}