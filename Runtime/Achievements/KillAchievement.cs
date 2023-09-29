using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Events;

namespace DarkBestiary.Achievements
{
    public class KillAchievement : Achievement
    {
        public KillAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            base.Subscribe();
            HealthComponent.AnyEntityDied += OnEntityDied;
        }

        public override void Unsubscribe()
        {
            base.Unsubscribe();
            HealthComponent.AnyEntityDied -= OnEntityDied;
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.GetComponent<UnitComponent>().Id != Data.UnitId)
            {
                return;
            }

            AddQuantity();
        }
    }
}