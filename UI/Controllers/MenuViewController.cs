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
            this.settingsController = ViewControllerRegistry.Initialize<SettingsViewController>();
            this.feedbackController = ViewControllerRegistry.Initialize<FeedbackViewController>();

            View.EnterSettings += ViewOnEnterSettings;
            View.EnterFeedback += ViewOnEnterFeedback;
            View.EnterMainMenu += ViewOnEnterMainMenu;
            View.EnterTown += ViewOnEnterTown;
            View.ExitGame += ViewOnExitGame;
        }

        protected override void OnViewHidden()
        {
            this.settingsController.View.Hide();
            this.feedbackController.View.Hide();
        }

        protected override void OnTerminate()
        {
            this.settingsController.Terminate();
            this.feedbackController.Terminate();

            View.EnterSettings -= ViewOnEnterSettings;
            View.EnterFeedback -= ViewOnEnterFeedback;
            View.EnterMainMenu -= ViewOnEnterMainMenu;
            View.EnterTown -= ViewOnEnterTown;
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

        private void ViewOnEnterTown()
        {
            Game.Instance.ToHub();
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