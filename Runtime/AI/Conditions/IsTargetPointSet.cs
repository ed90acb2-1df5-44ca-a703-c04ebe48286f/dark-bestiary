using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointSet : BehaviourTreeLogicNode
    {
        public IsTargetPointSet(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return context.TargetPoint != null ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}