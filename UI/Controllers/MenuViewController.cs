using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class MenuViewController : ViewController<IMenuView>
    {
        private SettingsViewController settingsController;
        private FeedbackViewController feedbackController;

        public MenuViewController(IMenuView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            this.settingsController = Container.Instance.Instantiate<SettingsViewController>();
            this.settingsController.Initialize();
            this.settingsController.View.Hide();

            this.feedbackController = Container.Instance.Instantiate<FeedbackViewController>();
            this.feedbackController.Initialize();
            this.feedbackController.View.Hide();

            View.EnterSettings += ViewOnEnterSettings;
            View.EnterFeedback += ViewOnEnterFeedback;
            View.EnterMainMenu += ViewOnEnterMainMenu;
            View.ExitGame += ViewOnExitGame;
        }

        protected override void OnTerminate()
        {
            this.settingsController.Terminate();
            this.feedbackController.Terminate();

            View.EnterSettings -= ViewOnEnterSettings;
            View.EnterFeedback -= ViewOnEnterFeedback;
            View.EnterMainMenu -= ViewOnEnterMainMenu;
            View.ExitGame -= ViewOnExitGame;
        }

        private void ViewOnEnterFeedback()
        {
            this.feedbackController.View.Show();
        }

        private void ViewOnEnterSettings()
        {
            this.settingsController.View.Show();
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