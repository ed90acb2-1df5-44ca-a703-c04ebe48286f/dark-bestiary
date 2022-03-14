using DarkBestiary.Items;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ScenarioViewController : ViewController<IScenarioView>
    {
        private readonly Scenario scenario;

        public ScenarioViewController(IScenarioView view, Scenario scenario) : base(view)
        {
            this.scenario = scenario;
        }

        protected override void OnInitialize()
        {
            View.ReturnToTown += OnReturnToTown;
            View.RewardChosen += OnRewardChosen;
            View.ClaimReward += OnClaimReward;
            View.Construct(this.scenario);
        }

        protected override void OnTerminate()
        {
            View.ReturnToTown -= OnReturnToTown;
            View.RewardChosen -= OnRewardChosen;
            View.ClaimReward -= OnClaimReward;
        }

        private void OnRewardChosen(Item item)
        {
            this.scenario.ChooseReward(item);
        }

        private void OnClaimReward()
        {
            this.scenario.ClaimRewards();
        }

        private void OnReturnToTown()
        {
            if (!this.scenario.IsEnd || this.scenario.IsFailed)
            {
                Game.Instance.ToHub();
                return;
            }

            Game.Instance.ToOutro();
        }
    }
}