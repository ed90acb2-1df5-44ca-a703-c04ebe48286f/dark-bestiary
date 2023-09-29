using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ForgottenDepthsViewController : ViewController<IForgottenDepthsView>
    {
        private readonly ForgottenDepthsManager m_Manager;

        public ForgottenDepthsViewController(ForgottenDepthsManager manager, IForgottenDepthsView view) : base(view)
        {
            m_Manager = manager;
        }

        protected override void OnInitialize()
        {
            View.StartButtonClicked += OnStartButtonClicked;

            // Manager recalculates behaviours when scenario is completed
            // looks like controller creates before this event.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                View.Construct(m_Manager.Depth, m_Manager.GetMonsterLevel(), m_Manager.Behaviours);
            });
        }

        private void OnStartButtonClicked()
        {
            m_Manager.LaunchScenario();
        }
    }
}