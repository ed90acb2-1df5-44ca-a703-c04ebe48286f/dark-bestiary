using System;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MainMenuView : View, IMainMenuView
    {
        public event Action? CampaignButtonClicked;
        public event Action? SettingsButtonClicked;
        public event Action? KeyBindingsButtonClicked;
        public event Action? CreditsButtonClicked;
        public event Action? QuitButtonClicked;

        [SerializeField] private Interactable m_CampaignButton = null!;
        [SerializeField] private Interactable m_SettingsButton = null!;
        [SerializeField] private Interactable m_KeyBindingsButton = null!;
        [SerializeField] private Interactable m_CreditsButton = null!;
        [SerializeField] private Interactable m_QuitButton = null!;

        protected override void OnInitialize()
        {
            m_CampaignButton.PointerClick += OnCampaignButtonClicked;
            m_SettingsButton.PointerClick += OnSettingsButtonPointerClick;
            m_KeyBindingsButton.PointerClick += OnKeyBindingsButtonPointerClick;
            m_CreditsButton.PointerClick += OnCreditsButtonPointerClick;
            m_QuitButton.PointerClick += OnQuitButtonPointerClick;
        }

        private void OnCampaignButtonClicked()
        {
            CampaignButtonClicked?.Invoke();
        }

        private void OnKeyBindingsButtonPointerClick()
        {
            KeyBindingsButtonClicked?.Invoke();
        }

        private void OnSettingsButtonPointerClick()
        {
            SettingsButtonClicked?.Invoke();
        }

        private void OnCreditsButtonPointerClick()
        {
            CreditsButtonClicked?.Invoke();
        }

        private void OnQuitButtonPointerClick()
        {
            QuitButtonClicked?.Invoke();
        }
    }
}