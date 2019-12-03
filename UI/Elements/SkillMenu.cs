using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillMenu : MonoBehaviour
    {
        public event Payload<Skill> Selected;

        [SerializeField] private SkillMenuButton skillButtonPrefab;
        [SerializeField] private Transform skillButtonContainer;
        [SerializeField] private Interactable button;
        [SerializeField] private Image arrow;
        [SerializeField] private Sprite arrowUp;
        [SerializeField] private Sprite arrowDown;

        private readonly List<SkillMenuButton> skillButtons = new List<SkillMenuButton>();

        private void Start()
        {
            this.button.PointerUp += OnToggleContainer;
        }

        public void Refresh(List<Skill> skills)
        {
            foreach (var skillButton in this.skillButtons)
            {
                skillButton.Clicked -= OnSkillButtonClicked;
                Destroy(skillButton.gameObject);
            }

            this.skillButtons.Clear();

            foreach (var skill in skills)
            {
                var skillButton = Instantiate(this.skillButtonPrefab, this.skillButtonContainer);
                skillButton.Construct(skill);
                skillButton.Clicked += OnSkillButtonClicked;

                this.skillButtons.Add(skillButton);
            }
        }

        private void OnToggleContainer()
        {
            this.skillButtonContainer.gameObject.SetActive(!this.skillButtonContainer.gameObject.activeSelf);
            this.arrow.sprite = this.skillButtonContainer.gameObject.activeSelf ? this.arrowDown : this.arrowUp;
        }

        private void OnSkillButtonClicked(SkillMenuButton skillButton)
        {
            OnToggleContainer();
            Selected?.Invoke(skillButton.Skill);
        }
    }
}