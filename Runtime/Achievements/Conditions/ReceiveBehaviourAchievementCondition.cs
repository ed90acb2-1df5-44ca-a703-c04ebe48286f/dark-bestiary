using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.Achievements.Conditions
{
    public class ReceiveBehaviourAchievementCondition : AchievementCondition
    {
        public ReceiveBehaviourAchievementCondition(AchievementConditionData data) : base(data)
        {
        }

        public override void Subscribe()
        {
            BehavioursComponent.AnyBehaviourApplied += OnAnyBehaviourApplied;
        }

        public override void Unsubscribe()
        {
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourApplied;
        }

        private void OnAnyBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour.Id != Data.BehaviourId || behaviour.Target != Game.Instance.Character.Entity)
            {
                return;
            }

            AddQuantity();
        }
    }
}