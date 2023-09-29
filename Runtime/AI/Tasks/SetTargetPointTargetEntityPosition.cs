using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointTargetEntityPosition : BehaviourTreeLogicNode
    {
        public SetTargetPointTargetEntityPosition(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            context.TargetPoint = context.RequireTargetEntity().transform.position;
            return BehaviourTreeStatus.Success;
        }
    }
}