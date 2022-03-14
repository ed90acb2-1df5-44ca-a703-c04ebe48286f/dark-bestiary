using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class BestiaryView : View, IBestiaryView
    {
        public event Payload<UnitData> Selected;
        public event Payload<int> LevelChanged;

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TMP_InputField levelInput;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private BehaviourView behaviourPrefab;
        [SerializeField] private Transform behaviourContainer;
        [SerializeField] private ObjectWithText unitRowTitlePrefab;
        [SerializeField] private BestiaryUnitRow unitRowPrefab;
        [SerializeField] private Transform unitRowContainer;
        [SerializeField] private BestiaryAttributeRow attributeRowPrefab;
        [SerializeField] private Transform attributeRowContainer;
        [SerializeField] private GameObject propertyRowSeparatorPrefab;
        [SerializeField] private CharacterInfoRow propertyRowPrefab;
        [SerializeField] private Transform propertyRowContainer;
        [SerializeField] private SkillSlotView spellbookSlotPrefab;
        [SerializeField] private Transform spellbookSlotContainer;

        private BestiaryUnitRow selected;
        private MonoBehaviourPool<BehaviourView> behaviourPool;
        private MonoBehaviourPool<SkillSlotView> spellbookSlotPool;
        private MonoBehaviourPool<CharacterInfoRow> propertyRowPool;
        private MonoBehaviourPool<BestiaryAttributeRow> attributeRowPool;
        private MonoBehaviourPool<ObjectWithText> unitRowTitlePool;
        private MonoBehaviourPool<BestiaryUnitRow> unitRowPool;
        private GameObjectPool propertyRowSeparatorPool;

        public void Construct(List<UnitData> units, int level)
        {
            this.closeButton.PointerClick += Hide;
            this.levelInput.onValueChanged.AddListener(OnLevelInputChanged);

            this.unitRowPool = MonoBehaviourPool<BestiaryUnitRow>.Factory(
                this.unitRowPrefab, this.unitRowContainer
            );

            this.unitRowTitlePool = MonoBehaviourPool<ObjectWithText>.Factory(
                this.unitRowTitlePrefab, this.unitRowContainer
            );

            this.behaviourPool = MonoBehaviourPool<BehaviourView>.Factory(
                this.behaviourPrefab, this.behaviourContainer
            );

            this.spellbookSlotPool = MonoBehaviourPool<SkillSlotView>.Factory(
                this.spellbookSlotPrefab, this.spellbookSlotContainer
            );

            this.propertyRowPool = MonoBehaviourPool<CharacterInfoRow>.Factory(
                this.propertyRowPrefab, this.propertyRowContainer
            );

            this.attributeRowPool = MonoBehaviourPool<BestiaryAttributeRow>.Factory(
                this.attributeRowPrefab, this.attributeRowContainer
            );

            this.propertyRowSeparatorPool = GameObjectPool.Factory(
                this.propertyRowSeparatorPrefab, this.propertyRowContainer
            );

            this.levelInput.text = level.ToString();
            ClearRows();
            CreateRows(units);
            SelectFirstRow();
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerClick -= Hide;
            this.levelInput.onValueChanged.RemoveListener(OnLevelInputChanged);

            this.unitRowPool.Clear();
            this.unitRowTitlePool.Clear();
            this.behaviourPool.Clear();
            this.spellbookSlotPool.Clear();
            this.propertyRowPool.Clear();
            this.attributeRowPool.Clear();
            this.propertyRowSeparatorPool.Clear();
        }

        private void ClearRows()
        {
            this.unitRowTitlePool.DespawnAll();
            this.unitRowPool.DespawnAll();
        }

        private void CreateRows(IEnumerable<UnitData> units)
        {
            var grouped = units.OrderBy(u => u.Environment.Index).GroupBy(u => u.Environment.Id);

            foreach (var group in grouped)
            {
                this.unitRowTitlePool.Spawn().ChangeTitle(I18N.Instance.Get(group.First().Environment.NameKey));

                foreach (var unit in group.OrderBy(u => u.NameKey))
                {
                    var unitRow = this.unitRowPool.Spawn();
                    unitRow.Clicked += OnUnitRowClicked;
                    unitRow.Construct(unit);
                }
            }
        }

        private void OnLevelInputChanged(string value)
        {
            var level = 1;

            try
            {
                level = int.Parse(value);
            } catch (Exception exception)
            {
                // ignored
            }

            LevelChanged?.Invoke(level);
        }

        private void SelectFirstRow()
        {
            var firstRow = this.unitRowContainer.GetComponentsInChildren<BestiaryUnitRow>().FirstOrDefault();

            if (firstRow == null)
            {
                return;
            }

            OnUnitRowClicked(firstRow);
        }

        private void OnUnitRowClicked(BestiaryUnitRow row)
        {
            if (this.selected != null)
            {
                this.selected.Deselect();
            }

            this.selected = row;
            this.selected.Select();

            Selected?.Invoke(row.Unit);
        }

        public void Display(UnitComponent unit)
        {
            this.nameText.text = unit.Name;

            this.spellbookSlotPool.DespawnAll();

            foreach (var slot in unit.GetComponent<SpellbookComponent>().Slots
                .Where(s => s.SkillType == SkillType.Common && !s.Skill.IsEmpty()))
            {
                var slotView = this.spellbookSlotPool.Spawn();
                slotView.HideHotkey();
                slotView.DisableDrag();
                slotView.Initialize(slot);
            }

            this.attributeRowPool.DespawnAll();

            foreach (var attribute in unit.GetComponent<AttributesComponent>().Attributes.Values.OrderBy(a => a.Index))
            {
                this.attributeRowPool.Spawn().Initialize(attribute);
            }

            this.behaviourPool.DespawnAll();

            foreach (var behaviour in unit.GetComponent<BehavioursComponent>()
                .Behaviours.Where(b => b.Flags.HasFlag(BehaviourFlags.MonsterModifier)))
            {
                var behaviourView = this.behaviourPool.Spawn();
                behaviourView.Initialize(behaviour);
            }

            RefreshProperties(unit);
        }

        public void RefreshProperties(UnitComponent unit)
        {
            this.propertyRowPool.DespawnAll();
            this.propertyRowSeparatorPool.DespawnAll();

            this.propertyRowPool.Spawn()
                .Construct(I18N.Instance.Get("ui_challenge_rating"), unit.ChallengeRating.ToString());

            this.propertyRowSeparatorPool.Spawn();

            var properties = unit.GetComponent<PropertiesComponent>();

            CreatePropertyRow(properties.Get(PropertyType.AttackPower));
            CreatePropertyRow(properties.Get(PropertyType.SpellPower));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitChance));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitDamage));
            CreatePropertyRow(properties.Get(PropertyType.ArmorPenetration));
            CreatePropertyRow(properties.Get(PropertyType.MagicPenetration));
            CreatePropertyRow(properties.Get(PropertyType.Vampirism));

            this.propertyRowSeparatorPool.Spawn();

            CreatePropertyRow(properties.Get(PropertyType.Health));
            CreatePropertyRow(properties.Get(PropertyType.HealthRegeneration));
            CreatePropertyRow(properties.Get(PropertyType.Thorns));
            CreatePropertyRow(properties.Get(PropertyType.IncomingHealingIncrease));
            CreatePropertyRow(properties.Get(PropertyType.BlockChance));
            CreatePropertyRow(properties.Get(PropertyType.Dodge));
            CreatePropertyRow(properties.Get(PropertyType.DamageReflection));

            this.propertyRowSeparatorPool.Spawn();

            CreatePropertyRow(properties.Get(PropertyType.HealingIncrease));
            CreatePropertyRow(properties.Get(PropertyType.DamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.PhysicalDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.MeleeDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.RangedDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.MagicalDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.FireDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.ColdDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.LightningDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.ShadowDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.ArcaneDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.HolyDamageIncrease));
            CreatePropertyRow(properties.Get(PropertyType.PoisonDamageIncrease));

            this.propertyRowSeparatorPool.Spawn();

            CreatePropertyRow(properties.Get(PropertyType.CrushingResistance));
            CreatePropertyRow(properties.Get(PropertyType.SlashingResistance));
            CreatePropertyRow(properties.Get(PropertyType.PiercingResistance));
            CreatePropertyRow(properties.Get(PropertyType.FireResistance));
            CreatePropertyRow(properties.Get(PropertyType.ColdResistance));
            CreatePropertyRow(properties.Get(PropertyType.HolyResistance));
            CreatePropertyRow(properties.Get(PropertyType.ShadowResistance));
            CreatePropertyRow(properties.Get(PropertyType.ArcaneResistance));
            CreatePropertyRow(properties.Get(PropertyType.LightningResistance));
            CreatePropertyRow(properties.Get(PropertyType.PoisonResistance));

            this.propertyRowSeparatorPool.Spawn();

            CreatePropertyRow(properties.Get(PropertyType.IncomingDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingPhysicalDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingMagicalDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingCrushingDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingSlashingDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingPiercingDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingFireDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingColdDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingHolyDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingShadowDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingArcaneDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingLightningDamageReduction));
            CreatePropertyRow(properties.Get(PropertyType.IncomingPoisonDamageReduction));
        }

        private void CreatePropertyRow(Property property)
        {
            var row = this.propertyRowPool.Spawn();
            row.SetLabel(property.Name);
            row.SetValue(property.ValueString());
        }
    }
}