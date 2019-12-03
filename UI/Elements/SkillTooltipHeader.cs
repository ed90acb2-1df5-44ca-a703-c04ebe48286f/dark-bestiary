using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltipHeader : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Construct(Skill skill)
        {
            this.nameText.text = GetSkillNameWithCategory(skill);
            this.icon.sprite = Resources.Load<Sprite>(skill.Icon);
        }

        private static string GetSkillNameWithCategory(Skill skill)
        {
            var nameWithCategory = skill.Name.ToString();

            if (skill.Category != null)
            {
                nameWithCategory +=  "\n<size=70%><color=#ccc>" + skill.Category.Name;
            }

            return nameWithCategory;
        }
    }
}