using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityStunned : BehaviourTreeLogicNode
    {
        public IsTargetEntityStunned(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var target = context.TargetEntity;

            return target.GetComponent<BehavioursComponent>().IsStunned
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}