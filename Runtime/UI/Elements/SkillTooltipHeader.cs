using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltipHeader : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_NameText;

        public void Construct(Skill skill)
        {
            m_NameText.text = GetSkillNameWithCategory(skill);
            m_Icon.sprite = Resources.Load<Sprite>(skill.Icon);
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