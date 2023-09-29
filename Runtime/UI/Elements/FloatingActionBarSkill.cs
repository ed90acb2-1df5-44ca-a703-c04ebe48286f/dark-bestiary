using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class FloatingActionBarSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Skill Skill { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_CooldownText;
        [SerializeField] private Image m_CooldownImage;

        private bool m_IsHovered;

        public void Initialize(Skill skill)
        {
            Skill = skill;
            Skill.CooldownStarted += SkillCooldownUpdated;
            Skill.CooldownUpdated += SkillCooldownUpdated;
            Skill.CooldownFinished += SkillCooldownUpdated;

            m_Icon.sprite = Resources.Load<Sprite>(skill.Icon);

            SkillCooldownUpdated(Skill);
        }

        private void SkillCooldownUpdated(Skill skill)
        {
            if (skill.RemainingCooldown == 0)
            {
                m_CooldownText.text = "";
                m_CooldownImage.fillAmount = 0;
            }
            else
            {
                m_CooldownText.text = skill.RemainingCooldown.ToString();
                m_CooldownImage.fillAmount = (float) skill.RemainingCooldown / skill.Cooldown;
            }
        }

        public void Terminate()
        {
            Skill.CooldownStarted -= SkillCooldownUpdated;
            Skill.CooldownUpdated -= SkillCooldownUpdated;
            Skill.CooldownFinished -= SkillCooldownUpdated;
        }

        public void OnHide()
        {
            if (m_IsHovered)
            {
                SkillTooltip.Instance.Hide();
            }
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
            m_IsHovered = true;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillTooltip.Instance.Hide();
            m_IsHovered = false;
        }
    }
}