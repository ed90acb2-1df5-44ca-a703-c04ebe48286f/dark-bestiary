using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntitySelf : BehaviourTreeLogicNode
    {
        public SetTargetEntitySelf(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            context.TargetEntity = context.Entity;
            return BehaviourTreeStatus.Success;
        }
    }
}