using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class MainMenuView : View, IMainMenuView
    {
        public event Payload PlayButtonClicked;
        public event Payload CreateCharacterButtonClicked;
        public event Payload SettingsButtonClicked;
        public event Payload QuitButtonClicked;

        [SerializeField] private Interactable playButton;
        [SerializeField] private Interactable createCharacterButton;
        [SerializeField] private Interactable settingsButton;
        [SerializeField] private Interactable quitButton;
        [SerializeField] private TextMeshProUGUI versionText;

        protected override void OnInitialize()
        {
            this.playButton.PointerUp += OnPlayButtonPointerUp;
            this.createCharacterButton.PointerUp += OnCreateCharacterButtonPointerUp;
            this.settingsButton.PointerUp += OnSettingsButtonPointerUp;
            this.quitButton.PointerUp += OnQuitButtonPointerUp;
            this.versionText.text = Game.Instance.Version;
        }

        public void ShowPlayButton()
        {
            this.playButton.gameObject.SetActive(true);
        }

        public void HidePlayButton()
        {
            this.playButton.gameObject.SetActive(false);
        }

        private void OnCreateCharacterButtonPointerUp()
        {
            CreateCharacterButtonClicked?.Invoke();
        }

        private void OnPlayButtonPointerUp()
        {
            PlayButtonClicked?.Invoke();
        }

        private void OnSettingsButtonPointerUp()
        {
            SettingsButtonClicked?.Invoke();
        }

        private void OnQuitButtonPointerUp()
        {
            QuitButtonClicked?.Invoke();
        }
    }
}