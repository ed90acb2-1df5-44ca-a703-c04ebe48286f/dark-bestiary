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
        public event Payload ExitGame;

        [SerializeField] private Interactable exitButton;
        [SerializeField] private Interactable mainMenuButton;
        [SerializeField] private Interactable settingsButton;
        [SerializeField] private Interactable feedbackButton;
        [SerializeField] private Interactable closeButton;

        protected override void OnInitialize()
        {
            this.settingsButton.PointerUp += SettingsButtonOnPointerUp;
            this.mainMenuButton.PointerUp += MainMenuButtonOnPointerUp;
            this.feedbackButton.PointerUp += FeedbackButtonOnPointerUp;
            this.exitButton.PointerUp += ExitButtonOnPointerUp;
            this.closeButton.PointerUp += CloseButtonOnPointerUp;
        }

        protected override void OnTerminate()
        {
            this.settingsButton.PointerUp -= SettingsButtonOnPointerUp;
            this.mainMenuButton.PointerUp -= MainMenuButtonOnPointerUp;
            this.feedbackButton.PointerUp -= FeedbackButtonOnPointerUp;
            this.exitButton.PointerUp -= ExitButtonOnPointerUp;
            this.closeButton.PointerUp -= CloseButtonOnPointerUp;
        }

        private void FeedbackButtonOnPointerUp()
        {
            Hide();
            EnterFeedback?.Invoke();
        }

        private void CloseButtonOnPointerUp()
        {
            Hide();
        }

        private void ExitButtonOnPointerUp()
        {
            Hide();
            ExitGame?.Invoke();
        }

        private void MainMenuButtonOnPointerUp()
        {
            Hide();
            EnterMainMenu?.Invoke();
        }

        private void SettingsButtonOnPointerUp()
        {
            Hide();
            EnterSettings?.Invoke();
        }
    }
}