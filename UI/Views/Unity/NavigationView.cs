using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class NavigationView : View, INavigationView
    {
        public event Payload ToggleAchievements;
        public event Payload ToggleSpecializations;
        public event Payload ToggleMasteries;
        public event Payload ToggleAttributes;
        public event Payload ToggleEquipment;
        public event Payload ToggleReliquary;
        public event Payload ToggleCombatLog;
        public event Payload ToggleSkills;
        public event Payload ToggleTalents;
        public event Payload ToggleMail;
        public event Payload ToggleMenu;

        [SerializeField] private NavigationViewButton equipmentButton;
        [SerializeField] private NavigationViewButton reliquaryButton;
        [SerializeField] private NavigationViewButton masteriesButton;
        [SerializeField] private NavigationViewButton attributesButton;
        [SerializeField] private NavigationViewButton specializationsButton;
        [SerializeField] private NavigationViewButton skillsButton;
        [SerializeField] private NavigationViewButton talentsButton;
        [SerializeField] private NavigationViewButton achievementsButton;
        [SerializeField] private NavigationViewButton combatLogButton;
        [SerializeField] private NavigationViewButton mailButton;
        [SerializeField] private NavigationViewButton menuButton;

        protected override void OnInitialize()
        {
            this.achievementsButton.PointerClick += OnAchievementsButtonClicked;
            this.achievementsButton.ChangeHotkey(KeyBindings.Get(KeyType.Achievements));

            this.specializationsButton.PointerClick += OnSpecializationsButtonClicked;
            this.specializationsButton.ChangeHotkey(KeyBindings.Get(KeyType.Specializations));

            this.masteriesButton.PointerClick += OnMasteriesButtonClicked;
            this.masteriesButton.ChangeHotkey(KeyBindings.Get(KeyType.Masteries));

            this.attributesButton.PointerClick += OnAttributesButtonClicked;
            this.attributesButton.ChangeHotkey(KeyBindings.Get(KeyType.Attributes));

            this.equipmentButton.PointerClick += OnEquipmentButtonClicked;
            this.equipmentButton.ChangeHotkey(KeyBindings.Get(KeyType.Equipment));

            this.reliquaryButton.PointerClick += OnReliquaryButtonClicked;
            this.reliquaryButton.ChangeHotkey(KeyBindings.Get(KeyType.Reliquary));

            this.combatLogButton.PointerClick += OnCombatLogButtonClicked;
            this.combatLogButton.ChangeHotkey(KeyBindings.Get(KeyType.CombatLog));

            this.skillsButton.PointerClick += OnSkillsButtonClicked;
            this.skillsButton.ChangeHotkey(KeyBindings.Get(KeyType.Skills));

            this.talentsButton.PointerClick += OnTalentsButtonClicked;
            this.talentsButton.ChangeHotkey(KeyBindings.Get(KeyType.Talents));

            this.mailButton.PointerClick += OnMailButtonClicked;
            this.mailButton.ChangeHotkey(KeyBindings.Get(KeyType.Mailbox));

            this.menuButton.PointerClick += OnMenuButtonClicked;
            this.menuButton.ChangeHotkey(KeyBindings.Get(KeyType.Menu));

            if (Game.Instance.IsVisions)
            {
                this.mailButton.gameObject.SetActive(false);
            }

            if (Game.Instance.IsVisions)
            {
                this.specializationsButton.gameObject.SetActive(false);
            }
        }

        public void HighlightSpecializationsButton()
        {
            this.specializationsButton.Highlight();
        }

        public void UnhighlightSpecializationsButton()
        {
            this.specializationsButton.Unhighlight();
        }

        public void HighlightReliquaryButton()
        {
            this.reliquaryButton.Highlight();
        }

        public void UnhighlightReliquaryButton()
        {
            this.reliquaryButton.Unhighlight();
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

        private void OnSpecializationsButtonClicked()
        {
            ToggleSpecializations?.Invoke();
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

        private void OnReliquaryButtonClicked()
        {
            ToggleReliquary?.Invoke();
        }

        private void OnEquipmentButtonClicked()
        {
            ToggleEquipment?.Invoke();
        }
    }
}