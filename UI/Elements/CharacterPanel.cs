using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Properties;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.UI.Elements
{
    public class CharacterPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private Image experienceFiller;
        [SerializeField] private CharacterInfoRow valueRowPrefab;
        [SerializeField] private CharacterInfoPanelSkillSlot skillSlotPrefab;
        [SerializeField] private GameObject separatorPrefab;
        [SerializeField] private GameObject attributesContent;
        [SerializeField] private GameObject skillSlotsContent;
        [SerializeField] private RectTransform attributesContainer;
        [SerializeField] private RectTransform skillSlotsContainer;
        [SerializeField] private Toggle helmCheckbox;
        [SerializeField] private TMP_Dropdown dropdown;

        [Header("Attribute value colors")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color negativeColor;
        [SerializeField] private Color positiveColor;

        private AttributesComponent attributes;
        private PropertiesComponent properties;
        private ExperienceComponent experience;

        private List<CharacterInfoPanelSkillSlot> skillSlotRows;
        private Dictionary<AttributeType, CharacterInfoRow> attributeRows;
        private Dictionary<PropertyType, CharacterInfoRow> propertyRows;

        private bool requiresUpdate;

        public void Initialize(Character character)
        {
            this.attributes = character.Entity.GetComponent<AttributesComponent>();
            this.attributes.AttributeChanged += OnAttributeChanged;

            this.properties = character.Entity.GetComponent<PropertiesComponent>();
            this.properties.PropertyChanged += OnPropertyChanged;

            this.experience = character.Entity.GetComponent<ExperienceComponent>();
            this.experience.Experience.Changed += OnExperienceChanged;
            OnExperienceChanged(this.experience.Experience);

            var actor = character.Entity.GetComponent<ActorComponent>();

            this.helmCheckbox.isOn = actor.IsHelmVisible;
            this.helmCheckbox.onValueChanged.AddListener(b => actor.SetHelmVisible(b));

            this.characterNameText.text = character.Name;

            this.dropdown.onValueChanged.AddListener(OnDropdownChanged);
            this.dropdown.options = new List<TMP_Dropdown.OptionData>
            {
                new TMP_Dropdown.OptionData(I18N.Instance.Get("ui_attributes")),
                new TMP_Dropdown.OptionData(I18N.Instance.Get("ui_skills")),
            };

            CreateAttributes(this.attributes);
            CreateProperties(this.properties);
            CreateSkillSlots(character.Entity.GetComponent<SpellbookComponent>().Slots);

            OnDropdownChanged(0);
        }

        public void Terminate()
        {
            this.attributes.AttributeChanged -= OnAttributeChanged;
            this.properties.PropertyChanged -= OnPropertyChanged;
            this.experience.Experience.Changed -= OnExperienceChanged;

            foreach (var skillSlotRow in this.skillSlotRows)
            {
                skillSlotRow.Terminate();
            }
        }

        private void CreateAttributes(AttributesComponent attributes)
        {
            this.attributeRows = new Dictionary<AttributeType, CharacterInfoRow>();

            foreach (var attribute in attributes.Attributes.Values.OrderBy(a => a.Index))
            {
                var row = Instantiate(this.valueRowPrefab, this.attributesContainer);
                this.attributeRows.Add(attribute.Type, row);
                RefreshAttributeRow(attribute, row);
            }
        }

        private void CreateProperties(PropertiesComponent properties)
        {
            this.propertyRows = new Dictionary<PropertyType, CharacterInfoRow>();

            Instantiate(this.separatorPrefab, this.attributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.AttackPower));
            CreatePropertyRow(properties.Get(PropertyType.SpellPower));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitChance));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitDamage));
            CreatePropertyRow(properties.Get(PropertyType.ArmorPenetration));
            CreatePropertyRow(properties.Get(PropertyType.MagicPenetration));
            CreatePropertyRow(properties.Get(PropertyType.Vampirism));

            Instantiate(this.separatorPrefab, this.attributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.Health));
            CreatePropertyRow(properties.Get(PropertyType.HealthRegeneration));
            CreatePropertyRow(properties.Get(PropertyType.Thorns));
            CreatePropertyRow(properties.Get(PropertyType.IncomingHealingIncrease));
            CreatePropertyRow(properties.Get(PropertyType.BlockChance));
            CreatePropertyRow(properties.Get(PropertyType.BlockAmount));
            CreatePropertyRow(properties.Get(PropertyType.Dodge));
            CreatePropertyRow(properties.Get(PropertyType.DamageReflection));

            Instantiate(this.separatorPrefab, this.attributesContainer);

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

            Instantiate(this.separatorPrefab, this.attributesContainer);

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

            Instantiate(this.separatorPrefab, this.attributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.MinionDamage));
            CreatePropertyRow(properties.Get(PropertyType.MinionHealth));

            Instantiate(this.separatorPrefab, this.attributesContainer);

            CreatePropertyRow(properties.Get(PropertyType.Experience));
        }

        private void CreatePropertyRow(Property property)
        {
            var row = Instantiate(this.valueRowPrefab, this.attributesContainer);
            this.propertyRows.Add(property.Type, row);

            RefreshPropertyRow(property, row);
        }

        private void CreateSkillSlots(List<SkillSlot> slots)
        {
            this.skillSlotRows = new List<CharacterInfoPanelSkillSlot>();

            foreach (var slot in slots)
            {
                var slotRow = Instantiate(this.skillSlotPrefab, this.skillSlotsContainer);
                slotRow.Initialize(slot);
                this.skillSlotRows.Add(slotRow);
            }
        }

        private void OnDropdownChanged(int value)
        {
            this.attributesContent.SetActive(value == 0);
            this.skillSlotsContent.SetActive(value == 1);
        }

        private void OnExperienceChanged(Experience experience)
        {
            this.levelText.text = $"{I18N.Instance.Get("ui_level")} {experience.Level}";
            this.experienceText.text = $"{experience.GetObtained()}/{experience.GetRequired()}";
            this.experienceFiller.fillAmount = experience.GetObtainedFraction();
        }

        private void OnAttributeChanged(AttributesComponent attributes, Attribute attribute)
        {
            this.requiresUpdate = true;
        }

        private void OnPropertyChanged(PropertiesComponent properties, Property property)
        {
            this.requiresUpdate = true;
        }

        private void RefreshRows()
        {
            foreach (var attribute in this.attributes.Attributes.Values)
            {
                if (!this.attributeRows.ContainsKey(attribute.Type))
                {
                    continue;
                }

                RefreshAttributeRow(attribute, this.attributeRows[attribute.Type]);
            }

            foreach (var property in this.properties.Properties.Values)
            {
                if (!this.propertyRows.ContainsKey(property.Type))
                {
                    continue;
                }

                RefreshPropertyRow(property, this.propertyRows[property.Type]);
            }

            foreach (var skillSlotRow in this.skillSlotRows)
            {
                skillSlotRow.Refresh();
            }
        }

        private void RefreshAttributeRow(Attribute attribute, CharacterInfoRow row)
        {
            row.SetLabel(attribute.Name);
            row.SetValue(((int) attribute.Value()).ToString());
            row.SetTooltip(attribute.Description);

            if (attribute.HasNegativeModifiers())
            {
                row.SetValueColor(this.negativeColor);
                return;
            }

            row.SetValueColor(attribute.HasModifiers() ? this.positiveColor : this.defaultColor);
        }

        private void RefreshPropertyRow(Property property, CharacterInfoRow row)
        {
            row.SetLabel(property.Name);
            row.SetValue(property.ValueString());

            if (property.HasNegativeModifiers())
            {
                row.SetValueColor(this.negativeColor);
            }
            else
            {
                row.SetValueColor(property.HasModifiers() ? this.positiveColor : this.defaultColor);
            }
        }

        private void Update()
        {
            if (!this.requiresUpdate)
            {
                return;
            }

            this.requiresUpdate = false;
            RefreshRows();
        }
    }
}