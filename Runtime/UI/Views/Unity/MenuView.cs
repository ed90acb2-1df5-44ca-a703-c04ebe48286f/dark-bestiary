using System;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MenuView : View, IMenuView
    {
        public event Action EnterSettings;
        public event Action EnterMainMenu;
        public event Action EnterTown;
        public event Action ExitGame;

        [SerializeField] private Interactable m_ExitButton;
        [SerializeField] private Interactable m_MainMenuButton;
        [SerializeField] private Interactable m_TownButton;
        [SerializeField] private Interactable m_SettingsButton;
        [SerializeField] private Interactable m_CloseButton;

        protected override void OnInitialize()
        {
            m_SettingsButton.PointerClick += SettingsButtonOnPointerClick;
            m_MainMenuButton.PointerClick += MainMenuButtonOnPointerClick;
            m_TownButton.PointerClick += TownButtonOnPointerClick;
            m_ExitButton.PointerClick += ExitButtonOnPointerClick;
            m_CloseButton.PointerClick += CloseButtonOnPointerClick;
        }

        protected override void OnTerminate()
        {
            m_SettingsButton.PointerClick -= SettingsButtonOnPointerClick;
            m_MainMenuButton.PointerClick -= MainMenuButtonOnPointerClick;
            m_ExitButton.PointerClick -= ExitButtonOnPointerClick;
            m_CloseButton.PointerClick -= CloseButtonOnPointerClick;
        }

        private void Exit()
        {
            Hide();
            ExitGame?.Invoke();
        }

        private void ToMainMenu()
        {
            Hide();
            EnterMainMenu?.Invoke();
        }

        private void CloseButtonOnPointerClick()
        {
            Hide();
        }

        private void ExitButtonOnPointerClick()
        {
            Exit();
        }

        private void TownButtonOnPointerClick()
        {
            Hide();
            EnterTown?.Invoke();
        }

        private void MainMenuButtonOnPointerClick()
        {
            ToMainMenu();
        }

        private void SettingsButtonOnPointerClick()
        {
            Hide();
            EnterSettings?.Invoke();
        }
    }
}