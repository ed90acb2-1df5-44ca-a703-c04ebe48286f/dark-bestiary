using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityUndead : BehaviourTreeLogicNode
    {
        public IsTargetEntityUndead(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (context.TargetEntity == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return context.TargetEntity.GetComponent<UnitComponent>().IsUndead
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}