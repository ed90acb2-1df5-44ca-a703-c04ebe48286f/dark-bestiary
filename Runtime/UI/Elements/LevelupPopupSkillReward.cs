using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopupSkillReward : LevelupPopupReward
    {
        public Skill Skill { get; set; }

        [SerializeField] private Interactable m_Hover;

        private void Start()
        {
            if (m_Hover == null)
            {
                return;
            }

            m_Hover.PointerEnter += OnPointerEnter;
            m_Hover.PointerExit += OnPointerExit;
        }

        private void OnPointerEnter()
        {
            if (Skill == null)
            {
                return;
            }

            SkillTooltip.Instance.Show(Skill, GetComponent<RectTransform>());
        }

        private void OnPointerExit()
        {
            if (Skill == null)
            {
                return;
            }

            SkillTooltip.Instance.Hide();
        }
    }
}