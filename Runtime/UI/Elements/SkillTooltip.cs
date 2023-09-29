using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Skills;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltip : Singleton<SkillTooltip>
    {
        [SerializeField]
        private SkillTooltipHeader m_HeaderPrefab = null!;

        [SerializeField]
        private SkillTooltipCost m_CostPrefab = null!;

        [SerializeField]
        private SkillTooltipSet m_SetPrefab = null!;

        [SerializeField]
        private CustomText m_TextPrefab = null!;

        [SerializeField]
        private GameObject m_SeparatorPrefab = null!;

        [SerializeField]
        private Transform m_Container = null!;

        private RectTransform m_RectTransform;
        private RectTransform m_ParentRectTransform;
        private SkillTooltipHeader m_Header;
        private SkillTooltipCost m_Cost;
        private MonoBehaviourPool<CustomText> m_TextPool;
        private MonoBehaviourPool<SkillTooltipSet> m_SetPool;
        private GameObjectPool m_SeparatorPool;
        private bool m_IsInitialized;

        private void Start()
        {
            Initialize();
            Instance.Hide();
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_IsInitialized = true;

            m_TextPool = MonoBehaviourPool<CustomText>.Factory(m_TextPrefab, m_Container);
            m_SetPool = MonoBehaviourPool<SkillTooltipSet>.Factory(m_SetPrefab, m_Container);
            m_SeparatorPool = GameObjectPool.Factory(m_SeparatorPrefab, m_Container);

            m_RectTransform = GetComponent<RectTransform>();
            m_ParentRectTransform = m_RectTransform.parent.GetComponent<RectTransform>();
        }

        public void Terminate()
        {
            m_TextPool.Clear();
            m_SetPool.Clear();
            m_SeparatorPool.Clear();
        }

        public void Show(Skill skill, RectTransform rect)
        {
            Clear();

            gameObject.SetActive(true);

            CreateInfo(skill);
            CreateDescription(skill);
            CreateLore(skill);
            CreateCost(skill);
            CreateRequirements(skill);

            if (skill.Rarity?.Type == RarityType.Legendary)
            {
                CreateText().Text = " ";

                var text = CreateText();
                text.Color = Color.red;
                text.Text = I18N.Instance.Translate("exception_only_one_ultimate");
            }

            CreateSets(skill);

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);

            m_RectTransform.MoveTooltip(rect, m_ParentRectTransform);
            m_RectTransform.ClampPositionToParent();
        }

        private void CreateSets(Skill skill)
        {
            if (skill.Sets.Count == 0)
            {
                return;
            }

            CreateText().Text = " ";
            CreateSeparator();
            CreateText().Text = " ";

            foreach (var set in skill.Sets)
            {
                CreateSet().Construct(set, skill.Caster.GetComponent<SpellbookComponent>());
                CreateText().Text = " ";
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);
        }

        private void CreateRequirements(Skill skill)
        {
            if (!skill.HaveEquipmentRequirements() && !skill.RequiresDualWielding())
            {
                return;
            }

            CreateText().Text = " ";

            if (skill.HaveEquipmentRequirements())
            {
                var text = CreateText();
                text.Color = skill.EquipmentRequirementsMet() ? Color.white : Color.red;
                text.Text = $"{I18N.Instance.Get("ui_requires")}: {skill.RequiredItemCategory.Name}";
            }

            if (skill.RequiresDualWielding())
            {
                var text = CreateText();
                text.Color = skill.DualWieldRequirementMet() ? Color.white : Color.red;
                text.Text = $"{I18N.Instance.Get("ui_requires")}: {I18N.Instance.Get("ui_dual_wield")}";
            }
        }

        private void CreateCost(Skill skill)
        {
            var actionPoints = skill.GetCost().GetValueOrDefault(ResourceType.ActionPoint, 0);
            var rage = skill.GetCost().GetValueOrDefault(ResourceType.Rage, 0);

            if (actionPoints < 1 && rage < 1 && skill.Cooldown == 0)
            {
                if (m_Cost != null)
                {
                    m_Cost.gameObject.SetActive(false);
                }

                return;
            }

            CreateText().Text = " ";

            if (m_Cost == null)
            {
                m_Cost = Instantiate(m_CostPrefab, m_Container);
            }

            m_Cost.gameObject.SetActive(true);
            m_Cost.Refresh(skill);
            m_Cost.transform.SetAsLastSibling();
        }

        private void CreateLore(Skill skill)
        {
            if (skill.Lore.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";

            var lore = CreateText();
            lore.Color = new Color(0.9f, 0.8f, 0.5f);
            lore.Text = skill.Lore;
        }

        private void CreateDescription(Skill skill)
        {
            if (skill.Description.IsNullOrEmpty())
            {
                return;
            }

            CreateText().Text = " ";
            CreateText().Text = skill.Description.ToString(new StringVariableContext(skill.Caster, skill));
        }

        private void CreateInfo(Skill skill)
        {
            if (m_Header == null)
            {
                m_Header = Instantiate(m_HeaderPrefab, m_Container);
            }

            m_Header.Construct(skill);

            CreateText().Text = " ";
            CreateText().Text = $"{I18N.Instance.Get("ui_target")}: {skill.UseStrategy.Name}";
            CreateText().Text = $"{I18N.Instance.Get("ui_range")}: {skill.GetRangeString()}";

            if (skill.Aoe > 0)
            {
                CreateText().Text = $"{I18N.Instance.Get("ui_area")}: {EnumTranslator.Translate(skill.AoeShape)}";
                CreateText().Text = $"{I18N.Instance.Get("ui_radius")}: {skill.Aoe}";
            }
        }

        private CustomText CreateText()
        {
            var text = m_TextPool.Spawn();
            text.Style = FontStyles.Normal;
            text.Alignment = TextAlignmentOptions.MidlineLeft;
            text.Color = Color.white;

            return text;
        }

        private SkillTooltipSet CreateSet()
        {
            return m_SetPool.Spawn();
        }

        private GameObject CreateSeparator()
        {
            return m_SeparatorPool.Spawn();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Clear()
        {
            m_SeparatorPool.DespawnAll();
            m_SetPool.DespawnAll();
            m_TextPool.DespawnAll();
        }
    }
}