using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityToPrevious : BehaviourTreeLogicNode
    {
        public SetTargetEntityToPrevious(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            context.TargetEntity = context.PreviousTargetEntity;
            return BehaviourTreeStatus.Success;
        }
    }
}