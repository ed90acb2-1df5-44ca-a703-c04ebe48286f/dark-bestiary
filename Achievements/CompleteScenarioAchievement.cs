using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Data;
using DarkBestiary.Scenarios;

namespace DarkBestiary.Achievements
{
    public class CompleteScenarioAchievement : Achievement
    {
        public CompleteScenarioAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            Scenario.AnyScenarioCompleted += OnAnyScenarioCompleted;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            Scenario.AnyScenarioCompleted -= OnAnyScenarioCompleted;
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            if (scenario.Id != this.Data.ScenarioId)
            {
                return;
            }

            AddQuantity();
        }
    }
}