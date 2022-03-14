using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Data;
using DarkBestiary.Managers;

namespace DarkBestiary.Achievements
{
    public class CompleteTowerLevelAchievement : Achievement
    {
        public CompleteTowerLevelAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            TowerManager.FloorCompleted += OnTowerFloorCompleted;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            TowerManager.FloorCompleted -= OnTowerFloorCompleted;
        }

        private void OnTowerFloorCompleted(int floor)
        {
            if (floor != this.Data.Level)
            {
                return;
            }

            AddQuantity();
        }
    }
}