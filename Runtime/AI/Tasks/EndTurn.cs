using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class EndTurn : BehaviourTreeLogicNode
    {
        public EndTurn(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            context.Combat.EndTurn(context.Entity);
            return BehaviourTreeStatus.Success;
        }
    }
}