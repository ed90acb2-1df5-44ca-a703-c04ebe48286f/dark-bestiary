using DarkBestiary.Data;
using DarkBestiary.Extensions;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityAlly : BehaviourTreeLogicNode
    {
        public IsTargetEntityAlly(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (context.TargetEntity == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return context.TargetEntity.IsAllyOf(context.Entity)
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}