using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SpellbookSkill : PoolableMonoBehaviour
    {
        public Skill Skill { get; private set; } = null!;

        [SerializeField]
        private Image m_Icon = null!;

        [SerializeField]
        private Image m_UltimateBorder = null!;

        [SerializeField]
        private TextMeshProUGUI m_NameText = null!;

        [SerializeField]
        private RectTransform m_SkillTransform = null!;

        [SerializeField]
        private Interactable m_SkillHover = null!;

        [SerializeField]
        private DraggableSkill m_DraggableSkill = null!;

        [SerializeField]
        private CircleIcon m_CircleIconPrefab = null!;

        [SerializeField]
        private Transform m_CircleIconContainer = null!;

        private readonly List<CircleIcon> m_CircleIconViews = new();

        public void Construct(Skill skill)
        {
            Skill = skill;

            m_DraggableSkill.Change(Skill);

            m_Icon.sprite = Resources.Load<Sprite>(Skill.Icon);
            m_NameText.text = skill.Name;
            m_UltimateBorder.gameObject.SetActive(Skill.Rarity?.Type == RarityType.Legendary);

            m_SkillHover.PointerEnter += OnSkillPointerEnter;
            m_SkillHover.PointerExit += OnSkillPointerExit;

            foreach (var circleIconView in m_CircleIconViews)
            {
                Destroy(circleIconView.gameObject);
            }

            m_CircleIconViews.Clear();

            foreach (var set in Skill.Sets)
            {
                var circleIconView = Instantiate(m_CircleIconPrefab, m_CircleIconContainer);
                circleIconView.Construct(Resources.Load<Sprite>(set.Icon), set.Name);
                m_CircleIconViews.Add(circleIconView);
            }
        }

        private void OnSkillPointerEnter()
        {
            SkillTooltip.Instance.Show(Skill, m_SkillTransform);
        }

        private void OnSkillPointerExit()
        {
            SkillTooltip.Instance.Hide();
        }
    }
}