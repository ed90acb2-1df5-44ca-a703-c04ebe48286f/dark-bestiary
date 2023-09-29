using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Events;
using DarkBestiary.Extensions;

namespace DarkBestiary.Achievements.Conditions
{
    public class EnemySuicideAchievementCondition : AchievementCondition
    {
        public EnemySuicideAchievementCondition(AchievementConditionData data) : base(data)
        {
        }

        public override void Subscribe()
        {
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
        }

        public override void Unsubscribe()
        {
            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (data.Victim.GetComponent<UnitComponent>().Id != Data.UnitId || !data.Victim.IsEnemyOfPlayer() ||
                data.Victim != data.Killer)
            {
                return;
            }

            AddQuantity();
        }
    }
}