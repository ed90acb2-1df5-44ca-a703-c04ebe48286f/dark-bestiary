using DarkBestiary.Skills;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltipCost : PoolableMonoBehaviour
    {
        [SerializeField] private GameObject m_ActionPointsContainer;
        [SerializeField] private TextMeshProUGUI m_ActionPointsText;
        [SerializeField] private GameObject m_RageContainer;
        [SerializeField] private TextMeshProUGUI m_RageText;
        [SerializeField] private TextMeshProUGUI m_CooldownText;

        public void Refresh(Skill skill)
        {
            var actionPoints = skill.GetCost(ResourceType.ActionPoint);
            var rage = skill.GetCost(ResourceType.Rage);
            var cooldown = skill.Cooldown;

            m_ActionPointsContainer.gameObject.SetActive(actionPoints > 0);
            m_ActionPointsText.text = ((int) actionPoints).ToString();

            m_RageContainer.gameObject.SetActive(rage > 0);
            m_RageText.text = ((int) rage).ToString();

            m_CooldownText.text = cooldown == 0 ? " " : $"{I18N.Instance.Get("ui_cooldown")}: {cooldown}";
        }
    }
}