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

        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private Image cooldownImage;

        private bool isHovered;

        public void Initialize(Skill skill)
        {
            Skill = skill;
            Skill.CooldownStarted += SkillCooldownUpdated;
            Skill.CooldownUpdated += SkillCooldownUpdated;
            Skill.CooldownFinished += SkillCooldownUpdated;

            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);

            SkillCooldownUpdated(Skill);
        }

        private void SkillCooldownUpdated(Skill skill)
        {
            if (skill.RemainingCooldown == 0)
            {
                this.cooldownText.text = "";
                this.cooldownImage.fillAmount = 0;
            }
            else
            {
                this.cooldownText.text = skill.RemainingCooldown.ToString();
                this.cooldownImage.fillAmount = (float) skill.RemainingCooldown / skill.Cooldown;
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
            if (this.isHovered)
            {
                SkillTooltip.Instance.Hide();
            }
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
            this.isHovered = true;
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            SkillTooltip.Instance.Hide();
            this.isHovered = false;
        }
    }
}