using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class MenuViewController : ViewController<IMenuView>
    {
        private SettingsViewController m_SettingsController;

        public MenuViewController(IMenuView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            m_SettingsController = Container.Instance.Instantiate<SettingsViewController>();
            m_SettingsController.Initialize();

            View.EnterSettings += ViewOnEnterSettings;
            View.EnterMainMenu += ViewOnEnterMainMenu;
            View.EnterTown += ViewOnEnterTown;
            View.ExitGame += ViewOnExitGame;
        }

        protected override void OnViewHidden()
        {
            m_SettingsController.View.Hide();
        }

        protected override void OnTerminate()
        {
            m_SettingsController.Terminate();

            View.EnterSettings -= ViewOnEnterSettings;
            View.EnterMainMenu -= ViewOnEnterMainMenu;
            View.EnterTown -= ViewOnEnterTown;
            View.ExitGame -= ViewOnExitGame;
        }

        private void ViewOnEnterSettings()
        {
            m_SettingsController.View.Show();
        }

        private void ViewOnEnterTown()
        {
            Game.Instance.ToTown();
        }

        private void ViewOnEnterMainMenu()
        {
            Game.Instance.ToMainMenu();
        }

        private void ViewOnExitGame()
        {
            Application.Quit();
        }
    }
}