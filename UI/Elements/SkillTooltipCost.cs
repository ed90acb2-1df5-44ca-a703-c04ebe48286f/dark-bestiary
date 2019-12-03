using DarkBestiary.Skills;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltipCost : PoolableMonoBehaviour
    {
        [SerializeField] private GameObject actionPointsContainer;
        [SerializeField] private TextMeshProUGUI actionPointsText;
        [SerializeField] private GameObject rageContainer;
        [SerializeField] private TextMeshProUGUI rageText;
        [SerializeField] private TextMeshProUGUI cooldownText;

        public void Refresh(Skill skill)
        {
            var actionPoints = skill.GetCost(ResourceType.ActionPoint);
            var rage = skill.GetCost(ResourceType.Rage);
            var cooldown = skill.Cooldown;

            this.actionPointsContainer.gameObject.SetActive(actionPoints > 0);
            this.actionPointsText.text = ((int) actionPoints).ToString();

            this.rageContainer.gameObject.SetActive(rage > 0);
            this.rageText.text = ((int) rage).ToString();

            this.cooldownText.text = cooldown == 0 ? " " : $"{I18N.Instance.Get("ui_cooldown")}: {cooldown}";
        }
    }
}