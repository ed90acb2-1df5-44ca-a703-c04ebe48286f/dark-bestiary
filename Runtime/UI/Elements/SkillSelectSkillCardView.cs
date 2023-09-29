using System;
using System.Linq;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillSelectSkillCardView : PoolableMonoBehaviour, IPointerClickHandler
    {
        public event Action<SkillSelectSkillCardView>? Clicked;

        public Skill Skill { get; private set; } = null!;

        [SerializeField]
        private TextMeshProUGUI m_NameText = null!;

        [SerializeField]
        private TextMeshProUGUI m_DescriptionText = null!;

        [SerializeField]
        private TextMeshProUGUI m_CooldownText = null!;

        [SerializeField]
        private Image m_IconImage = null!;

        [SerializeField]
        private TextWithIcon m_SkillSetPrefab = null!;

        [SerializeField]
        private Transform m_SkillSetContainer = null!;

        private MonoBehaviourPool<TextWithIcon>? m_SkillSetPool;

        public void Construct(Skill skill)
        {
            Skill = skill;

            m_NameText.text = skill.Name;
            m_DescriptionText.text = skill.Description.ToString(new StringVariableContext(skill.Caster, skill));
            m_CooldownText.text = skill.Cooldown > 0 ? $"{I18N.Instance.Translate("ui_cooldown")}: {skill.Cooldown.ToString()}" : "";
            m_IconImage.sprite = Resources.Load<Sprite>(skill.Icon);

            m_SkillSetPool?.DespawnAll();
            m_SkillSetPool ??= MonoBehaviourPool<TextWithIcon>.Factory(m_SkillSetPrefab, m_SkillSetContainer, 3);

            foreach (var skillSet in skill.Sets.OrderBy(x => x.Name.Key))
            {
                var textWithIcon = m_SkillSetPool.Spawn();
                textWithIcon.Text = skillSet.Name;
                textWithIcon.Icon.sprite = Resources.Load<Sprite>(skillSet.Icon);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke(this);
        }
    }
}