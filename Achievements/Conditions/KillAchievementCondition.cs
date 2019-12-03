using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;

namespace DarkBestiary.Achievements.Conditions
{
    public class KillAchievementCondition : AchievementCondition
    {
        public KillAchievementCondition(AchievementConditionData data) : base(data)
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
            if (data.Victim.GetComponent<UnitComponent>().Id != this.Data.UnitId)
            {
                return;
            }

            AddQuantity();
        }
    }
}