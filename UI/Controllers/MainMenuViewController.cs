using DarkBestiary.Data.Repositories;
using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;
using DarkBestiary.Visions;

namespace DarkBestiary.UI.Controllers
{
    public class MainMenuViewController : ViewController<IMainMenuView>
    {
        private readonly ICharacterDataRepository characterDataRepository;

        private SettingsViewController settingsController;
        private KeyBindingsViewController keyBindingsController;

        public MainMenuViewController(IMainMenuView view, ICharacterDataRepository characterDataRepository) : base(view)
        {
            this.characterDataRepository = characterDataRepository;
        }

        protected override void OnInitialize()
        {
            this.settingsController = ViewControllerRegistry.Initialize<SettingsViewController>();
            this.keyBindingsController = ViewControllerRegistry.Initialize<KeyBindingsViewController>();

            View.CampaignButtonClicked += OnCampaignButtonClicked;
            View.VisionsButtonClicked += OnVisionsButtonClicked;
            View.SettingsButtonClicked += OnSettingsButtonClicked;
            View.KeyBindingsButtonClicked += OnKeyBindingsButtonClicked;
            View.CreditsButtonClicked += OnCreditsButtonClicked;
            View.QuitButtonClicked += OnQuitButtonClicked;
        }

        protected override void OnTerminate()
        {
            this.settingsController.Terminate();

            View.CampaignButtonClicked -= OnCampaignButtonClicked;
            View.VisionsButtonClicked -= OnVisionsButtonClicked;
            View.SettingsButtonClicked -= OnSettingsButtonClicked;
            View.KeyBindingsButtonClicked -= OnKeyBindingsButtonClicked;
            View.CreditsButtonClicked -= OnCreditsButtonClicked;
            View.QuitButtonClicked -= OnQuitButtonClicked;
        }

        private void OnCampaignButtonClicked()
        {
            Game.Instance.SwitchState(() =>
            {
                Game.Instance.Mode = GameMode.Campaign;

                if (this.characterDataRepository.FindAll().Count == 0)
                {
                    return new CharacterCreationGameState();
                }

                return new CharacterSelectionGameState();
            }, true);
        }

        private void OnVisionsButtonClicked()
        {
            Game.Instance.Mode = GameMode.Visions;
            VisionManager.IsNewGame = true;

            if (VisionManager.IsSaveExists())
            {
                Game.Instance.ToVisionMenu();
            }
            else
            {
                Game.Instance.SwitchState(() => new CharacterCreationGameState(), true);
            }
        }

        private void OnSettingsButtonClicked()
        {
            this.settingsController.View.Show();
        }

        private void OnKeyBindingsButtonClicked()
        {
            this.keyBindingsController.View.Show();
        }

        private void OnCreditsButtonClicked()
        {
            Game.Instance.SwitchState(() => new CreditsGameState(), true);
        }

        private void OnQuitButtonClicked()
        {
            Game.Quit();
        }
    }
}