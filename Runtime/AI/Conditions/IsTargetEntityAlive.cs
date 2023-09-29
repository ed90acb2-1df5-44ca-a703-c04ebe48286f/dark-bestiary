using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityAlive : BehaviourTreeLogicNode
    {
        public IsTargetEntityAlive(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var target = context.TargetEntity;
            var health = target.GetComponent<HealthComponent>();

            if (health == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return health.IsAlive ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}