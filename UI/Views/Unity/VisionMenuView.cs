using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class VisionMenuView : View, IVisionMenuView
    {
        public event Payload ContinueButtonClicked;
        public event Payload NewGameButtonClicked;
        public event Payload BackButtonClicked;

        [SerializeField] private Interactable continueButton;
        [SerializeField] private Interactable newGameButton;
        [SerializeField] private Interactable backButton;

        protected override void OnInitialize()
        {
            this.continueButton.PointerClick += OnContinueButtonPointerClick;
            this.newGameButton.PointerClick += OnNewGameButtonPointerClick;
            this.backButton.PointerClick += OnBackButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            this.continueButton.PointerClick -= OnContinueButtonPointerClick;
            this.newGameButton.PointerClick -= OnNewGameButtonPointerClick;
            this.backButton.PointerClick -= OnBackButtonPointerClick;
        }

        private void OnContinueButtonPointerClick()
        {
            ContinueButtonClicked?.Invoke();
        }

        private void OnNewGameButtonPointerClick()
        {
            ConfirmationWindow.Instance.Confirmed += OnExitConfirmed;
            ConfirmationWindow.Instance.Cancelled += OnExitCancelled;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_vision_new_game_confirmation"),
                I18N.Instance.Get("ui_confirm")
            );
        }

        private void OnExitConfirmed()
        {
            OnExitCancelled();
            NewGameButtonClicked?.Invoke();
        }

        private void OnExitCancelled()
        {
            ConfirmationWindow.Instance.Confirmed -= OnExitConfirmed;
            ConfirmationWindow.Instance.Cancelled -= OnExitCancelled;
        }

        private void OnBackButtonPointerClick()
        {
            BackButtonClicked?.Invoke();
        }
    }
}