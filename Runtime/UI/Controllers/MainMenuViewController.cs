using DarkBestiary.Data.Repositories;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class MainMenuViewController : ViewController<IMainMenuView>
    {
        private readonly ICharacterDataRepository m_CharacterDataRepository;

        private SettingsViewController m_SettingsController;
        private KeyBindingsViewController m_KeyBindingsController;

        public MainMenuViewController(IMainMenuView view, ICharacterDataRepository characterDataRepository) : base(view)
        {
            m_CharacterDataRepository = characterDataRepository;
        }

        protected override void OnInitialize()
        {
            m_SettingsController = Container.Instance.Instantiate<SettingsViewController>();
            m_SettingsController.Initialize();

            m_KeyBindingsController = Container.Instance.Instantiate<KeyBindingsViewController>();
            m_KeyBindingsController.Initialize();

            View.CampaignButtonClicked += OnCampaignButtonClicked;
            View.SettingsButtonClicked += OnSettingsButtonClicked;
            View.KeyBindingsButtonClicked += OnKeyBindingsButtonClicked;
            View.CreditsButtonClicked += OnCreditsButtonClicked;
            View.QuitButtonClicked += OnQuitButtonClicked;
        }

        protected override void OnTerminate()
        {
            m_SettingsController.Terminate();

            View.CampaignButtonClicked -= OnCampaignButtonClicked;
            View.SettingsButtonClicked -= OnSettingsButtonClicked;
            View.KeyBindingsButtonClicked -= OnKeyBindingsButtonClicked;
            View.CreditsButtonClicked -= OnCreditsButtonClicked;
            View.QuitButtonClicked -= OnQuitButtonClicked;
        }

        private void OnCampaignButtonClicked()
        {
            if (m_CharacterDataRepository.FindAll().Count == 0)
            {
                Game.Instance.ToCharacterCreation();
            }
            else
            {
                Game.Instance.ToCharacterSelection();
            }
        }

        private void OnSettingsButtonClicked()
        {
            m_SettingsController.View.Show();
        }

        private void OnKeyBindingsButtonClicked()
        {
            m_KeyBindingsController.View.Show();
        }

        private void OnCreditsButtonClicked()
        {
            Game.Instance.ToCredits();
        }

        private void OnQuitButtonClicked()
        {
            Game.Quit();
        }
    }
}