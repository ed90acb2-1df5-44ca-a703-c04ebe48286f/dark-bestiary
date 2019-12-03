using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntitySlowed : BehaviourTreeLogicNode
    {
        public IsTargetEntitySlowed(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var target = context.TargetEntity;

            return target.GetComponent<BehavioursComponent>().IsSlowed
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}