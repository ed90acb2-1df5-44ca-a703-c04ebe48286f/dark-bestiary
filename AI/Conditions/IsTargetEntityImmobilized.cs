using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityImmobilized : BehaviourTreeLogicNode
    {
        public IsTargetEntityImmobilized(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var target = context.TargetEntity;

            return target.GetComponent<BehavioursComponent>().IsImmobilized
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}