using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Properties;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class BestiaryView : View, IBestiaryView
    {
        public event Action<UnitData> Selected;
        public event Action<int> LevelChanged;

        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TMP_InputField m_LevelInput;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private BehaviourView m_BehaviourPrefab;
        [SerializeField] private Transform m_BehaviourContainer;
        [SerializeField] private ObjectWithText m_UnitRowTitlePrefab;
        [SerializeField] private BestiaryUnitRow m_UnitRowPrefab;
        [SerializeField] private Transform m_UnitRowContainer;
        [SerializeField] private BestiaryAttributeRow m_AttributeRowPrefab;
        [SerializeField] private Transform m_AttributeRowContainer;
        [SerializeField] private GameObject m_PropertyRowSeparatorPrefab;
        [SerializeField] private CharacterInfoRow m_PropertyRowPrefab;
        [SerializeField] private Transform m_PropertyRowContainer;
        [SerializeField] private SkillSlotView m_SpellbookSlotPrefab;
        [SerializeField] private Transform m_SpellbookSlotContainer;

        private BestiaryUnitRow m_Selected;
        private MonoBehaviourPool<BehaviourView> m_BehaviourPool;
        private MonoBehaviourPool<SkillSlotView> m_SpellbookSlotPool;
        private MonoBehaviourPool<CharacterInfoRow> m_PropertyRowPool;
        private MonoBehaviourPool<BestiaryAttributeRow> m_AttributeRowPool;
        private MonoBehaviourPool<ObjectWithText> m_UnitRowTitlePool;
        private MonoBehaviourPool<BestiaryUnitRow> m_UnitRowPool;
        private GameObjectPool m_PropertyRowSeparatorPool;

        public void Construct(List<UnitData> units, int level)
        {
            m_CloseButton.PointerClick += Hide;
            m_LevelInput.onValueChanged.AddListener(OnLevelInputChanged);

            m_UnitRowPool = MonoBehaviourPool<BestiaryUnitRow>.Factory(
                m_UnitRowPrefab, m_UnitRowContainer
            );

            m_UnitRowTitlePool = MonoBehaviourPool<ObjectWithText>.Factory(
                m_UnitRowTitlePrefab, m_UnitRowContainer
            );

            m_BehaviourPool = MonoBehaviourPool<BehaviourView>.Factory(
                m_BehaviourPrefab, m_BehaviourContainer
            );

            m_SpellbookSlotPool = MonoBehaviourPool<SkillSlotView>.Factory(
                m_SpellbookSlotPrefab, m_SpellbookSlotContainer
            );

            m_PropertyRowPool = MonoBehaviourPool<CharacterInfoRow>.Factory(
                m_PropertyRowPrefab, m_PropertyRowContainer
            );

            m_AttributeRowPool = MonoBehaviourPool<BestiaryAttributeRow>.Factory(
                m_AttributeRowPrefab, m_AttributeRowContainer
            );

            m_PropertyRowSeparatorPool = GameObjectPool.Factory(
                m_PropertyRowSeparatorPrefab, m_PropertyRowContainer
            );

            m_LevelInput.text = level.ToString();
            ClearRows();
            CreateRows(units);
            SelectFirstRow();
        }

        protected override void OnTerminate()
        {
            m_CloseButton.PointerClick -= Hide;
            m_LevelInput.onValueChanged.RemoveListener(OnLevelInputChanged);

            m_UnitRowPool.Clear();
            m_UnitRowTitlePool.Clear();
            m_BehaviourPool.Clear();
            m_SpellbookSlotPool.Clear();
            m_PropertyRowPool.Clear();
            m_AttributeRowPool.Clear();
            m_PropertyRowSeparatorPool.Clear();
        }

        private void ClearRows()
        {
            m_UnitRowTitlePool.DespawnAll();
            m_UnitRowPool.DespawnAll();
        }

        private void CreateRows(IEnumerable<UnitData> units)
        {
            var grouped = units.OrderBy(u => u.Environment.Index).GroupBy(u => u.Environment.Id);

            foreach (var group in grouped)
            {
                m_UnitRowTitlePool.Spawn().ChangeTitle(I18N.Instance.Get(group.First().Environment.NameKey));

                foreach (var unit in group.OrderBy(u => u.NameKey))
                {
                    var unitRow = m_UnitRowPool.Spawn();
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
            var firstRow = m_UnitRowContainer.GetComponentsInChildren<BestiaryUnitRow>().FirstOrDefault();

            if (firstRow == null)
            {
                return;
            }

            OnUnitRowClicked(firstRow);
        }

        private void OnUnitRowClicked(BestiaryUnitRow row)
        {
            if (m_Selected != null)
            {
                m_Selected.Deselect();
            }

            m_Selected = row;
            m_Selected.Select();

            Selected?.Invoke(row.Unit);
        }

        public void Display(UnitComponent unit)
        {
            m_NameText.text = unit.Name;

            m_SpellbookSlotPool.DespawnAll();

            foreach (var slot in unit.GetComponent<SpellbookComponent>().Slots
                .Where(s => s.SkillType == SkillType.Common && !s.Skill.IsEmpty()))
            {
                var slotView = m_SpellbookSlotPool.Spawn();
                slotView.HideHotkey();
                slotView.DisableDrag();
                slotView.Initialize(slot);
            }

            m_AttributeRowPool.DespawnAll();

            foreach (var attribute in unit.GetComponent<AttributesComponent>().Attributes.Values.OrderBy(a => a.Index))
            {
                m_AttributeRowPool.Spawn().Initialize(attribute);
            }

            m_BehaviourPool.DespawnAll();

            foreach (var behaviour in unit.GetComponent<BehavioursComponent>()
                .Behaviours.Where(b => b.Flags.HasFlag(BehaviourFlags.MonsterModifier)))
            {
                var behaviourView = m_BehaviourPool.Spawn();
                behaviourView.Initialize(behaviour);
            }

            RefreshProperties(unit);
        }

        public void RefreshProperties(UnitComponent unit)
        {
            m_PropertyRowPool.DespawnAll();
            m_PropertyRowSeparatorPool.DespawnAll();

            m_PropertyRowPool.Spawn()
                .Construct(I18N.Instance.Get("ui_challenge_rating"), unit.ChallengeRating.ToString());

            m_PropertyRowSeparatorPool.Spawn();

            var properties = unit.GetComponent<PropertiesComponent>();

            CreatePropertyRow(properties.Get(PropertyType.AttackPower));
            CreatePropertyRow(properties.Get(PropertyType.SpellPower));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitChance));
            CreatePropertyRow(properties.Get(PropertyType.CriticalHitDamage));
            CreatePropertyRow(properties.Get(PropertyType.ArmorPenetration));
            CreatePropertyRow(properties.Get(PropertyType.MagicPenetration));
            CreatePropertyRow(properties.Get(PropertyType.Vampirism));

            m_PropertyRowSeparatorPool.Spawn();

            CreatePropertyRow(properties.Get(PropertyType.Health));
            CreatePropertyRow(properties.Get(PropertyType.HealthRegeneration));
            CreatePropertyRow(properties.Get(PropertyType.Thorns));
            CreatePropertyRow(properties.Get(PropertyType.IncomingHealingIncrease));
            CreatePropertyRow(properties.Get(PropertyType.BlockChance));
            CreatePropertyRow(properties.Get(PropertyType.Dodge));
            CreatePropertyRow(properties.Get(PropertyType.DamageReflection));

            m_PropertyRowSeparatorPool.Spawn();

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

            m_PropertyRowSeparatorPool.Spawn();

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

            m_PropertyRowSeparatorPool.Spawn();

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
            var row = m_PropertyRowPool.Spawn();
            row.SetLabel(property.Name);
            row.SetValue(property.ValueString());
        }
    }
}