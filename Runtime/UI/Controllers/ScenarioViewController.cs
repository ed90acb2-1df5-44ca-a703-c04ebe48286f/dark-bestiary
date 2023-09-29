using DarkBestiary.Items;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ScenarioViewController : ViewController<IScenarioView>
    {
        private readonly Scenario m_Scenario;

        public ScenarioViewController(IScenarioView view, Scenario scenario) : base(view)
        {
            m_Scenario = scenario;
        }

        protected override void OnInitialize()
        {
            View.ReturnToTown += OnReturnToTown;
            View.ReturnToMap += OnReturnToMap;
            View.RewardChosen += OnRewardChosen;
            View.ClaimReward += OnClaimReward;
            View.Construct(m_Scenario);
        }

        protected override void OnTerminate()
        {
            View.ReturnToTown -= OnReturnToTown;
            View.ReturnToMap += OnReturnToMap;
            View.RewardChosen -= OnRewardChosen;
            View.ClaimReward -= OnClaimReward;
        }

        private void OnRewardChosen(Item item)
        {
            m_Scenario.ChooseReward(item);
        }

        private void OnClaimReward()
        {
            m_Scenario.ClaimRewards();
        }

        private void OnReturnToMap()
        {
            Game.Instance.ToMap();
        }

        private void OnReturnToTown()
        {
            Game.Instance.ToTown();
        }
    }
}