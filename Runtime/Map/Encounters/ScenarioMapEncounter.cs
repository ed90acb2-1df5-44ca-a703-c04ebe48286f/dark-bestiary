using System;
using DarkBestiary.Data;
using DarkBestiary.Scenarios;

namespace DarkBestiary.Map.Encounters
{
    public class ScenarioMapEncounter : IMapEncounter
    {
        private readonly MapEncounterData m_Data;

        private Action? m_OnSuccess;
        private Action? m_OnFailure;

        public ScenarioMapEncounter(MapEncounterData data)
        {
            m_Data = data;
        }

        public void Run(Action onSuccess, Action onFailure)
        {
            m_OnSuccess = onSuccess;
            m_OnFailure = onFailure;

            Scenario.AnyScenarioCompleted += OnAnyScenarioCompleted;
            Scenario.AnyScenarioFailed += OnAnyScenarioFailed;
            Game.Instance.ToScenario(m_Data.ScenarioId);
        }

        public void Cleanup()
        {
            Scenario.AnyScenarioCompleted -= OnAnyScenarioCompleted;
            Scenario.AnyScenarioFailed -= OnAnyScenarioFailed;
        }

        private void OnAnyScenarioFailed(Scenario scenario)
        {
            m_OnFailure?.Invoke();
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            m_OnSuccess?.Invoke();
        }
    }
}