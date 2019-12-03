using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class ScenarioView : View, IScenarioView
    {
        public event Payload ReturnToTown;
        public event Payload ClaimReward;
        public event Payload<Item> RewardChosen;

        [SerializeField] private DefeatPanel defeatPanel;
        [SerializeField] private VictoryPanel victoryPanel;
        [SerializeField] private ScenarioProgress progress;

        private Scenario scenario;

        public void Construct(Scenario scenario)
        {
            this.scenario = scenario;
        }

        protected override void OnInitialize()
        {
            this.progress.Initialize(this.scenario);
            this.progress.gameObject.SetActive(this.scenario.Episodes.Count > 1);

            Scenario.AnyScenarioCompleted += OnScenarioAnyScenarioCompleted;
            Scenario.AnyScenarioFailed += OnScenarioAnyScenarioFailed;
        }

        protected override void OnTerminate()
        {
            this.progress.Terminate();

            Scenario.AnyScenarioCompleted -= OnScenarioAnyScenarioCompleted;
            Scenario.AnyScenarioFailed -= OnScenarioAnyScenarioFailed;
        }

        private void OnVictoryPanelChooseReward(VictoryPanelReward reward)
        {
            RewardChosen?.Invoke(reward.Item);
        }

        private void OnVictoryPanelComplete()
        {
            this.victoryPanel.Terminate();

            ClaimReward?.Invoke();
            ReturnToTown?.Invoke();
        }

        private void OnDefeatPanelReturnToTown()
        {
            this.defeatPanel.Terminate();

            ReturnToTown?.Invoke();
        }

        private void OnScenarioAnyScenarioFailed(Scenario scenario)
        {
            this.progress.gameObject.SetActive(false);

            Timer.Instance.Wait(2.0f, () =>
            {
                this.defeatPanel.gameObject.SetActive(true);
                this.defeatPanel.ReturnToTown += OnDefeatPanelReturnToTown;
                this.defeatPanel.Initialize(this.scenario);
            });
        }

        private void OnScenarioAnyScenarioCompleted(Scenario scenario)
        {
            this.progress.gameObject.SetActive(false);

            Timer.Instance.Wait(2.0f, () =>
            {
                this.victoryPanel.gameObject.SetActive(true);
                this.victoryPanel.Complete += OnVictoryPanelComplete;
                this.victoryPanel.ChooseReward += OnVictoryPanelChooseReward;
                this.victoryPanel.Initialize(this.scenario);
            });
        }
    }
}