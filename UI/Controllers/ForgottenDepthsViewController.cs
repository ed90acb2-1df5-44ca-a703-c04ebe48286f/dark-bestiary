using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ForgottenDepthsViewController : ViewController<IForgottenDepthsView>
    {
        private readonly ForgottenDepthsManager manager;

        public ForgottenDepthsViewController(ForgottenDepthsManager manager, IForgottenDepthsView view) : base(view)
        {
            this.manager = manager;
        }

        protected override void OnInitialize()
        {
            View.StartButtonClicked += OnStartButtonClicked;

            // Manager recalculates behaviours when scenario is completed
            // looks like controller creates before this event.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                View.Construct(this.manager.Depth, this.manager.GetMonsterLevel(), this.manager.Behaviours);
            });
        }

        private void OnStartButtonClicked()
        {
            this.manager.LaunchScenario();
        }
    }
}