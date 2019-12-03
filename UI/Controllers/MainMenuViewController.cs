using DarkBestiary.Data.Repositories;
using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class MainMenuViewController : ViewController<IMainMenuView>
    {
        private readonly ICharacterDataRepository characterDataRepository;

        private SettingsViewController settingsController;

        public MainMenuViewController(IMainMenuView view, ICharacterDataRepository characterDataRepository) : base(view)
        {
            this.characterDataRepository = characterDataRepository;
        }

        protected override void OnInitialize()
        {
            this.settingsController = Container.Instance.Instantiate<SettingsViewController>();
            this.settingsController.Initialize();
            this.settingsController.View.Hide();

            if (this.characterDataRepository.FindAll().Count == 0)
            {
                View.HidePlayButton();
            }
            else
            {
                View.ShowPlayButton();
            }

            View.PlayButtonClicked += OnPlayButtonClicked;
            View.CreateCharacterButtonClicked += OnCreateCharacterButtonClicked;
            View.SettingsButtonClicked += OnSettingsButtonClicked;
            View.QuitButtonClicked += OnQuitButtonClicked;
        }

        protected override void OnTerminate()
        {
            this.settingsController.Terminate();

            View.PlayButtonClicked -= OnPlayButtonClicked;
            View.SettingsButtonClicked -= OnSettingsButtonClicked;
            View.QuitButtonClicked -= OnQuitButtonClicked;
        }

        private void OnPlayButtonClicked()
        {
            Game.Instance.SwitchState(new CharacterSelectionGameState());
        }

        private void OnCreateCharacterButtonClicked()
        {
            Game.Instance.SwitchState(new CharacterCreationGameState());
        }

        private void OnSettingsButtonClicked()
        {
            this.settingsController.View.Show();
        }

        private void OnQuitButtonClicked()
        {
            Game.Quit();
        }
    }
}