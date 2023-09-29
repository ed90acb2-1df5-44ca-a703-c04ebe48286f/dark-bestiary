using System;
using DarkBestiary.Items;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class ScenarioView : View, IScenarioView
    {
        public event Action? ReturnToTown;
        public event Action? ReturnToMap;
        public event Action? ClaimReward;
        public event Action<Item>? RewardChosen;

        [SerializeField]
        private DefeatPanel m_DefeatPanel = null!;

        [SerializeField]
        private VictoryPanel m_VictoryPanel = null!;

        [SerializeField]
        private ScenarioProgress m_Progress = null!;

        private Scenario m_Scenario = null!;

        public void Construct(Scenario scenario)
        {
            m_Scenario = scenario;

            MaybeCreateProgressView();

            Scenario.AnyScenarioCompleted += OnScenarioAnyScenarioCompleted;
            Scenario.AnyScenarioFailed += OnScenarioAnyScenarioFailed;
        }

        protected override void OnTerminate()
        {
            m_Progress.Terminate();

            Scenario.AnyScenarioCompleted -= OnScenarioAnyScenarioCompleted;
            Scenario.AnyScenarioFailed -= OnScenarioAnyScenarioFailed;
        }

        private void MaybeCreateProgressView()
        {
            var showProgress = !m_Scenario.IsAscension && m_Scenario.Episodes.Count > 1;

            if (showProgress)
            {
                m_Progress.Initialize(m_Scenario);
            }

            m_Progress.gameObject.SetActive(showProgress);
        }

        private void OnScenarioAnyScenarioFailed(Scenario scenario)
        {
            m_Progress.gameObject.SetActive(false);

            Timer.Instance.Wait(2.0f, () =>
            {
                m_DefeatPanel.gameObject.SetActive(true);
                m_DefeatPanel.CompleteButtonClicked += OnDefeatPanelCompleteButtonClicked;
                m_DefeatPanel.Initialize(m_Scenario);
            });
        }

        private void OnScenarioAnyScenarioCompleted(Scenario scenario)
        {
            m_Progress.gameObject.SetActive(false);

            Timer.Instance.Wait(2.0f, () =>
            {
                m_VictoryPanel.gameObject.SetActive(true);
                m_VictoryPanel.CompleteButtonClicked += OnVictoryPanelCompleteButtonClicked;
                m_VictoryPanel.ChooseReward += OnVictoryPanelChooseReward;
                m_VictoryPanel.Initialize(m_Scenario);
            });
        }

        private void OnVictoryPanelChooseReward(VictoryPanelReward reward)
        {
            RewardChosen?.Invoke(reward.Item);
        }

        private void OnVictoryPanelCompleteButtonClicked()
        {
            m_VictoryPanel.Terminate();

            ClaimReward?.Invoke();
            ReturnToMap?.Invoke();
        }

        private void OnDefeatPanelCompleteButtonClicked()
        {
            m_DefeatPanel.Terminate();

            ReturnToTown?.Invoke();
        }
    }
}