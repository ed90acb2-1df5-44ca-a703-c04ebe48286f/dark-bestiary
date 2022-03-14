using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetHealthBelow : BehaviourTreeLogicNode
    {
        public IsTargetHealthBelow(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var health = context.RequireTargetEntity().GetComponent<HealthComponent>();

            return health.HealthFraction <= Properties.HealthFraction
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}