using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MenuView : View, IMenuView
    {
        public event Payload EnterSettings;
        public event Payload EnterFeedback;
        public event Payload EnterMainMenu;
        public event Payload EnterTown;
        public event Payload ExitGame;

        [SerializeField] private Interactable exitButton;
        [SerializeField] private Interactable mainMenuButton;
        [SerializeField] private Interactable townButton;
        [SerializeField] private Interactable settingsButton;
        [SerializeField] private Interactable feedbackButton;
        [SerializeField] private Interactable closeButton;

        protected override void OnInitialize()
        {
            if (Game.Instance.IsVisions)
            {
                this.townButton.gameObject.SetActive(false);
            }

            this.settingsButton.PointerClick += SettingsButtonOnPointerClick;
            this.mainMenuButton.PointerClick += MainMenuButtonOnPointerClick;
            this.townButton.PointerClick += TownButtonOnPointerClick;
            this.feedbackButton.PointerClick += FeedbackButtonOnPointerClick;
            this.exitButton.PointerClick += ExitButtonOnPointerClick;
            this.closeButton.PointerClick += CloseButtonOnPointerClick;
        }

        protected override void OnTerminate()
        {
            this.settingsButton.PointerClick -= SettingsButtonOnPointerClick;
            this.mainMenuButton.PointerClick -= MainMenuButtonOnPointerClick;
            this.feedbackButton.PointerClick -= FeedbackButtonOnPointerClick;
            this.exitButton.PointerClick -= ExitButtonOnPointerClick;
            this.closeButton.PointerClick -= CloseButtonOnPointerClick;
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

        private void FeedbackButtonOnPointerClick()
        {
            Hide();
            EnterFeedback?.Invoke();
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