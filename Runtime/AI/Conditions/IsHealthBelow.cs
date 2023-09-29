using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsHealthBelow : BehaviourTreeLogicNode
    {
        public IsHealthBelow(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var health = context.Entity.GetComponent<HealthComponent>();

            return health.HealthFraction <= Properties.HealthFraction
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}