using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class NavigationView : View, INavigationView
    {
        public event Payload ToggleAchievements;
        public event Payload ToggleMasteries;
        public event Payload ToggleAttributes;
        public event Payload ToggleEquipment;
        public event Payload ToggleCombatLog;
        public event Payload ToggleSkills;
        public event Payload ToggleTalents;
        public event Payload ToggleMail;
        public event Payload ToggleMenu;

        [SerializeField] private NavigationViewButton equipmentButton;
        [SerializeField] private NavigationViewButton masteriesButton;
        [SerializeField] private NavigationViewButton attributesButton;
        [SerializeField] private NavigationViewButton skillsButton;
        [SerializeField] private NavigationViewButton talentsButton;
        [SerializeField] private NavigationViewButton achievementsButton;
        [SerializeField] private NavigationViewButton combatLogButton;
        [SerializeField] private NavigationViewButton mailButton;
        [SerializeField] private NavigationViewButton menuButton;

        protected override void OnInitialize()
        {
            this.achievementsButton.PointerUp += OnAchievementsButtonClicked;
            this.masteriesButton.PointerUp += OnMasteriesButtonClicked;
            this.attributesButton.PointerUp += OnAttributesButtonClicked;
            this.equipmentButton.PointerUp += OnEquipmentButtonClicked;
            this.combatLogButton.PointerUp += OnCombatLogButtonClicked;
            this.skillsButton.PointerUp += OnSkillsButtonClicked;
            this.talentsButton.PointerUp += OnTalentsButtonClicked;
            this.mailButton.PointerUp += OnMailButtonClicked;
            this.menuButton.PointerUp += OnMenuButtonClicked;
        }

        public void HighlightTalentsButton()
        {
            this.talentsButton.Highlight();
        }

        public void UnhighlightTalentsButton()
        {
            this.talentsButton.Unhighlight();
        }

        public void HighlightAttributesButton()
        {
            this.attributesButton.Highlight();
        }

        public void UnhighlightAttributesButton()
        {
            this.attributesButton.Unhighlight();
        }

        public void HighlightMailButton()
        {
            this.mailButton.Highlight();
        }

        public void UnhighlightMailButton()
        {
            this.mailButton.Unhighlight();
        }

        private void OnMailButtonClicked()
        {
            ToggleMail?.Invoke();
        }

        private void OnAchievementsButtonClicked()
        {
            ToggleAchievements?.Invoke();
        }

        private void OnMasteriesButtonClicked()
        {
            ToggleMasteries?.Invoke();
        }

        private void OnAttributesButtonClicked()
        {
            ToggleAttributes?.Invoke();
        }

        private void OnMenuButtonClicked()
        {
            ToggleMenu?.Invoke();
        }

        private void OnTalentsButtonClicked()
        {
            ToggleTalents?.Invoke();
        }

        private void OnSkillsButtonClicked()
        {
            ToggleSkills?.Invoke();
        }

        private void OnCombatLogButtonClicked()
        {
            ToggleCombatLog?.Invoke();
        }

        private void OnEquipmentButtonClicked()
        {
            ToggleEquipment?.Invoke();
        }
    }
}