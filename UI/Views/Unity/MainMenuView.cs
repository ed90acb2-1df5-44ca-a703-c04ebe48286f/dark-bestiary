using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MainMenuView : View, IMainMenuView
    {
        public event Payload VisionsButtonClicked;
        public event Payload CampaignButtonClicked;
        public event Payload SettingsButtonClicked;
        public event Payload KeyBindingsButtonClicked;
        public event Payload CreditsButtonClicked;
        public event Payload QuitButtonClicked;

        [SerializeField] private Interactable campaignButton;
        [SerializeField] private Interactable visionsButton;
        [SerializeField] private Interactable settingsButton;
        [SerializeField] private Interactable keyBindingsButton;
        [SerializeField] private Interactable creditsButton;
        [SerializeField] private Interactable quitButton;

        protected override void OnInitialize()
        {
            this.campaignButton.PointerClick += OnCampaignButtonClicked;
            this.visionsButton.PointerClick += OnVisionsButtonClicked;
            this.settingsButton.PointerClick += OnSettingsButtonPointerClick;
            this.keyBindingsButton.PointerClick += OnKeyBindingsButtonPointerClick;
            this.creditsButton.PointerClick += OnCreditsButtonPointerClick;
            this.quitButton.PointerClick += OnQuitButtonPointerClick;
        }

        private void OnVisionsButtonClicked()
        {
            VisionsButtonClicked?.Invoke();
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