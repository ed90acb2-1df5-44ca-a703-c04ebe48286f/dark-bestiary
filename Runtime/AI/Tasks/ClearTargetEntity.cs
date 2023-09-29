using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class ClearTargetEntity : BehaviourTreeLogicNode
    {
        public ClearTargetEntity(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            context.TargetEntity = null;
            return BehaviourTreeStatus.Success;
        }
    }
}