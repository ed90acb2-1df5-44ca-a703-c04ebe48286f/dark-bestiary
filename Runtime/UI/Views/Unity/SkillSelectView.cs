using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Views.Unity
{
    public class SkillSelectView : View, ISkillSelectView
    {
        public event Action<Skill>? ContinueButtonClicked;
        public event Action? RefreshButtonClicked;

        [SerializeField]
        private SkillSelectSkillCardView m_SkillCardPrefab = null!;

        [SerializeField]
        private Transform m_SkillCardContainer = null!;

        [SerializeField]
        private SkillSelectSkillView m_SkillPrefab = null!;

        [SerializeField]
        private Transform m_SkillContainer = null!;

        [SerializeField]
        private SkillTooltipSet m_SkillSetPrefab = null!;

        [SerializeField]
        private RectTransform m_SkillSetContainer = null!;

        [SerializeField]
        private Interactable m_RefreshButton = null!;

        private MonoBehaviourPool<SkillSelectSkillView> m_SkillViewPool = null!;
        private MonoBehaviourPool<SkillSelectSkillCardView> m_SkillCardViewPool = null!;
        private MonoBehaviourPool<SkillTooltipSet> m_SkillSetViewPool = null!;

        private SpellbookComponent m_Spellbook = null!;
        private List<SkillSet> m_SkillSets = null!;

        private void Start()
        {
            m_RefreshButton.PointerClick += OnRefreshButtonPointerClick;
        }

        public void Construct(SpellbookComponent spellbook, List<SkillSet> skillSets)
        {
            m_Spellbook = spellbook;
            m_SkillSets = skillSets;

            m_SkillViewPool = MonoBehaviourPool<SkillSelectSkillView>.Factory(m_SkillPrefab, m_SkillContainer);
            m_SkillCardViewPool = MonoBehaviourPool<SkillSelectSkillCardView>.Factory(m_SkillCardPrefab, m_SkillCardContainer, 3);
            m_SkillSetViewPool = MonoBehaviourPool<SkillTooltipSet>.Factory(m_SkillSetPrefab, m_SkillSetContainer, skillSets.Count);
        }

        public void Refresh(List<Skill> skills)
        {
            CreateSkillSets();
            CreateSkillCards();
            CreateSkills();
            RebuildLayout();
            return;

            void CreateSkillSets()
            {
                m_SkillSetViewPool.DespawnAll();

                var skillSets = m_SkillSets
                    .OrderByDescending(m_Spellbook.GetSkillSetPiecesEquipped)
                    .ThenBy(x => x.Name.Key)
                    .ThenByDescending(x => x.SkillIds.Any(skillId => skills.Any(skill => skill.Id == skillId)))
                    .ThenBy(x => x.Name.Key);

                foreach (var skillSet in skillSets)
                {
                    m_SkillSetViewPool.Spawn().Construct(skillSet, m_Spellbook);
                }
            }

            void CreateSkillCards()
            {
                m_SkillCardViewPool.DespawnAll();

                foreach (var skill in skills)
                {
                    var skillView = m_SkillCardViewPool.Spawn();
                    skillView.Clicked -= OnSkillClicked;
                    skillView.Clicked += OnSkillClicked;
                    skillView.Construct(skill);
                }
            }

            void CreateSkills()
            {
                m_SkillViewPool.DespawnAll();

                foreach (var skill in m_Spellbook.Slots.Where(x => x.Skill.Type == SkillType.Common).Select(x => x.Skill))
                {
                    m_SkillViewPool.Spawn().Construct(skill);
                }
            }
        }

        private void RebuildLayout()
        {
            foreach (var rectTransform in GetComponentsInChildren<RectTransform>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }
        }

        private void OnSkillClicked(SkillSelectSkillCardView skillCardView)
        {
            ContinueButtonClicked?.Invoke(skillCardView.Skill);
            Hide();
        }

        private void OnRefreshButtonPointerClick()
        {
            m_RefreshButton.Active = false;
            RefreshButtonClicked?.Invoke();
        }

        private void OnEnable()
        {
            m_RefreshButton.Active = true;
        }
    }
}