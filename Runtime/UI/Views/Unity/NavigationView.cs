using System;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class NavigationView : View, INavigationView
    {
        public event Action? ToggleAchievements;
        public event Action? ToggleMasteries;
        public event Action? ToggleAttributes;
        public event Action? ToggleEquipment;
        public event Action? ToggleReliquary;
        public event Action? ToggleCombatLog;
        public event Action? ToggleDamageMeter;
        public event Action? ToggleTalents;
        public event Action? ToggleMail;
        public event Action? ToggleMenu;

        [SerializeField]
        private NavigationViewButton m_EquipmentButton = null!;

        [SerializeField]
        private NavigationViewButton m_ReliquaryButton = null!;

        [SerializeField]
        private NavigationViewButton m_MasteriesButton = null!;

        [SerializeField]
        private NavigationViewButton m_AttributesButton = null!;

        [SerializeField]
        private NavigationViewButton m_TalentsButton = null!;

        [SerializeField]
        private NavigationViewButton m_AchievementsButton = null!;

        [SerializeField]
        private NavigationViewButton m_CombatLogButton = null!;

        [SerializeField]
        private NavigationViewButton m_DamageMeterButton = null!;

        [SerializeField]
        private NavigationViewButton m_MailButton = null!;

        [SerializeField]
        private NavigationViewButton m_MenuButton = null!;

        protected override void OnInitialize()
        {
            m_AchievementsButton.PointerClick += OnAchievementsButtonClicked;
            m_AchievementsButton.ChangeHotkey(KeyBindings.Get(KeyType.Achievements));

            m_MasteriesButton.PointerClick += OnMasteriesButtonClicked;
            m_MasteriesButton.ChangeHotkey(KeyBindings.Get(KeyType.Masteries));

            m_AttributesButton.PointerClick += OnAttributesButtonClicked;
            m_AttributesButton.ChangeHotkey(KeyBindings.Get(KeyType.Attributes));

            m_EquipmentButton.PointerClick += OnEquipmentButtonClicked;
            m_EquipmentButton.ChangeHotkey(KeyBindings.Get(KeyType.Equipment));

            m_ReliquaryButton.PointerClick += OnReliquaryButtonClicked;
            m_ReliquaryButton.ChangeHotkey(KeyBindings.Get(KeyType.Reliquary));

            m_CombatLogButton.PointerClick += OnCombatLogButtonClicked;
            m_CombatLogButton.ChangeHotkey(KeyBindings.Get(KeyType.CombatLog));

            m_DamageMeterButton.PointerClick += OnDamageButtonClicked;
            m_DamageMeterButton.ChangeHotkey(KeyBindings.Get(KeyType.DamageMeter));

            m_TalentsButton.PointerClick += OnTalentsButtonClicked;
            m_TalentsButton.ChangeHotkey(KeyBindings.Get(KeyType.Talents));

            m_MailButton.PointerClick += OnMailButtonClicked;
            m_MailButton.ChangeHotkey(KeyBindings.Get(KeyType.Mailbox));

            m_MenuButton.PointerClick += OnMenuButtonClicked;
            m_MenuButton.ChangeHotkey(KeyBindings.Get(KeyType.Menu));
        }

        public void HighlightReliquaryButton()
        {
            m_ReliquaryButton.Highlight();
        }

        public void UnhighlightReliquaryButton()
        {
            m_ReliquaryButton.Unhighlight();
        }

        public void HighlightTalentsButton()
        {
            m_TalentsButton.Highlight();
        }

        public void UnhighlightTalentsButton()
        {
            m_TalentsButton.Unhighlight();
        }

        public void HighlightAttributesButton()
        {
            m_AttributesButton.Highlight();
        }

        public void UnhighlightAttributesButton()
        {
            m_AttributesButton.Unhighlight();
        }

        public void HighlightMailButton()
        {
            m_MailButton.Highlight();
        }

        public void UnhighlightMailButton()
        {
            m_MailButton.Unhighlight();
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

        private void OnCombatLogButtonClicked()
        {
            ToggleCombatLog?.Invoke();
        }

        private void OnDamageButtonClicked()
        {
            ToggleDamageMeter?.Invoke();
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