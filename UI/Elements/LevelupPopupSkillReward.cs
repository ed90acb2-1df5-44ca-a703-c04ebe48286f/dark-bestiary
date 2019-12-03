using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class LevelupPopupSkillReward : LevelupPopupReward
    {
        public Skill Skill { get; set; }

        [SerializeField] private Interactable hover;

        private void Start()
        {
            if (this.hover == null)
            {
                return;
            }

            this.hover.PointerEnter += OnPointerEnter;
            this.hover.PointerExit += OnPointerExit;
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