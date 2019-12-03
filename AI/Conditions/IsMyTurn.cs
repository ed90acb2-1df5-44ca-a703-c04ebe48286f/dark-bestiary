using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsMyTurn : BehaviourTreeLogicNode
    {
        public IsMyTurn(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (context.Combat == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return context.Combat.IsEntityTurn(context.Entity)
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}