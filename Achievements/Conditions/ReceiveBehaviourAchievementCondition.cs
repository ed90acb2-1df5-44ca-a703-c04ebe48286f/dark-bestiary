using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Managers;

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
            if (behaviour.Id != this.Data.BehaviourId || behaviour.Target != CharacterManager.Instance.Character.Entity)
            {
                return;
            }

            AddQuantity();
        }
    }
}