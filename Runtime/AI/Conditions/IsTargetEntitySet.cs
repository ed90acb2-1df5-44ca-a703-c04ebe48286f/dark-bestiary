using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntitySet : BehaviourTreeLogicNode
    {
        public IsTargetEntitySet(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return context.TargetEntity != null ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}