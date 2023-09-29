using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Properties;
using UnityEngine;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Elements
{
    public class CharacterAttributesPanel : MonoBehaviour
    {
        [SerializeField]
        private CharacterInfoRow m_ValueRowPrefab = null!;

        [SerializeField]
        private GameObject m_SeparatorPrefab = null!;

        [SerializeField]
        private RectTransform m_AttributesContainer = null!;

        [Header("Attribute value colors")]
        [SerializeField]
        private Color m_DefaultColor;

        [SerializeField]
        private Color m_NegativeColor;

        [SerializeField]
        private Color m_PositiveColor;

        private readonly Dictionary<AttributeType, CharacterInfoRow> m_AttributeRows = new();
        private readonly Dictionary<PropertyType, CharacterInfoRow> m_PropertyRows = new();

        private AttributesComponent m_Attributes = null!;
        private PropertiesComponent m_Properties = null!;

        private bool m_RequiresUpdate;

        public void Initialize(Character character)
        {
            m_Attributes = character.Entity.GetComponent<AttributesComponent>();
            m_Attributes.AttributeChanged += OnAttributeChanged;

            m_Properties = character.Entity.GetComponent<PropertiesComponent>();
            m_Properties.PropertyChanged += OnPropertyChanged;

            CreateAttributes(m_Attributes);
            CreateProperties(m_Properties);
        }

        public void Terminate()
        {
            m_Attributes.AttributeChanged -= OnAttributeChanged;
            m_Properties.PropertyChanged -= OnPropertyChanged;
        }

        private void CreateAttributes(AttributesComponent attributes)
        {
            foreach (var attribute in attributes.Attributes.Values.OrderBy(a => a.Index))
            {
                var row = Instantiate(m_ValueRowPrefab, m_AttributesContainer);
                m_AttributeRows.Add(attribute.Type, row);
                RefreshAttributeRow(attribute, row);
            }
        }

        private void CreateProperties(PropertiesComponent properties)
        {
            Instantiate(m_SeparatorPrefab, m_AttributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.AttackPower));
            CreatePropertyRow(properties.Get(PropertyType.SpellPower));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitChance));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitDamage));
            CreatePropertyRow(properties.Get(PropertyType.ArmorPenetration));
            CreatePropertyRow(properties.Get(PropertyType.MagicPenetration));
            CreatePropertyRow(properties.Get(PropertyType.Vampirism));

            Instantiate(m_SeparatorPrefab, m_AttributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.Health));
            CreatePropertyRow(properties.Get(PropertyType.HealthRegeneration));
            CreatePropertyRow(properties.Get(PropertyType.Thorns));
            CreatePropertyRow(properties.Get(PropertyType.IncomingHealingIncrease));
            CreatePropertyRow(properties.Get(PropertyType.BlockChance));
            CreatePropertyRow(properties.Get(PropertyType.BlockAmount));
            CreatePropertyRow(properties.Get(PropertyType.Dodge));
            CreatePropertyRow(properties.Get(PropertyType.DamageReflection));

            Instantiate(m_SeparatorPrefab, m_AttributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.HealingIncrease));
            CreatePropertyRow(properties.Get(PropertyType.DamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.PhysicalDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.MeleeDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.RangedDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.MagicalDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.CrushingDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.SlashingDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.PiercingDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.FireDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.ColdDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.LightningDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.ShadowDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.ArcaneDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.HolyDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.PoisonDamageIncrease));

            Instantiate(m_SeparatorPrefab, m_AttributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.IncomingDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingPhysicalDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.CrushingResistance));
            CreatePropertyRow(properties.Get(PropertyType.SlashingResistance));
            CreatePropertyRow(properties.Get(PropertyType.PiercingResistance));
            CreatePropertyRow(properties.Get(PropertyType.IncomingMagicalDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.FireResistance));
            CreatePropertyRow(properties.Get(PropertyType.ColdResistance));
            CreatePropertyRow(properties.Get(PropertyType.HolyResistance));
            CreatePropertyRow(properties.Get(PropertyType.ShadowResistance));
            CreatePropertyRow(properties.Get(PropertyType.ArcaneResistance));
            CreatePropertyRow(properties.Get(PropertyType.LightningResistance));
            CreatePropertyRow(properties.Get(PropertyType.PoisonResistance));

            Instantiate(m_SeparatorPrefab, m_AttributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.MinionDamage));
            CreatePropertyRow(properties.Get(PropertyType.MinionHealth));

            Instantiate(m_SeparatorPrefab, m_AttributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.Experience));
        }

        private void CreatePropertyRow(Property property)
        {
            var row = Instantiate(m_ValueRowPrefab, m_AttributesContainer);
            m_PropertyRows.Add(property.Type, row);

            RefreshPropertyRow(property, row);
        }

        private void OnAttributeChanged(AttributesComponent attributes, Attribute attribute)
        {
            m_RequiresUpdate = true;
        }

        private void OnPropertyChanged(PropertiesComponent properties, Property property)
        {
            m_RequiresUpdate = true;
        }

        private void RefreshRows()
        {
            foreach (var attribute in m_Attributes.Attributes.Values)
            {
                if (!m_AttributeRows.ContainsKey(attribute.Type))
                {
                    continue;
                }

                RefreshAttributeRow(attribute, m_AttributeRows[attribute.Type]);
            }

            foreach (var property in m_Properties.Properties.Values)
            {
                if (!m_PropertyRows.ContainsKey(property.Type))
                {
                    continue;
                }

                RefreshPropertyRow(property, m_PropertyRows[property.Type]);
            }
        }

        private void RefreshAttributeRow(Attribute attribute, CharacterInfoRow row)
        {
            row.SetLabel(attribute.Name);
            row.SetValue(((int) attribute.Value()).ToString());
            row.SetTooltip(attribute.Description);

            if (attribute.HasNegativeModifiers())
            {
                row.SetValueColor(m_NegativeColor);
                return;
            }

            row.SetValueColor(attribute.HasModifiers() ? m_PositiveColor : m_DefaultColor);
        }

        private void RefreshPropertyRow(Property property, CharacterInfoRow row)
        {
            row.SetLabel(property.Name);
            row.SetTooltip(property.Description.IsNullOrEmpty() ? property.Name : property.Description);
            row.SetValue(property.ValueString());

            if (property.HasNegativeModifiers())
            {
                row.SetValueColor(m_NegativeColor);
            }
            else
            {
                row.SetValueColor(property.HasModifiers() ? m_PositiveColor : m_DefaultColor);
            }
        }

        private void Update()
        {
            if (!m_RequiresUpdate)
            {
                return;
            }

            m_RequiresUpdate = false;
            RefreshRows();
        }
    }
}