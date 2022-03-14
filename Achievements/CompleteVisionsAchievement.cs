using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Data;
using DarkBestiary.Visions;

namespace DarkBestiary.Achievements
{
    public class CompleteVisionsAchievement : Achievement
    {
        public CompleteVisionsAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            VisionManager.Completed += OnVisionsCompleted;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            VisionManager.Completed -= OnVisionsCompleted;
        }

        private void OnVisionsCompleted()
        {
            AddQuantity();
        }
    }
}