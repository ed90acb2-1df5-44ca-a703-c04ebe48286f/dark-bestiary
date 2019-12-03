using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class CanMove : BehaviourTreeLogicNode
    {
        public CanMove(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var entity = context.Entity;
            var movement = entity.GetComponent<MovementComponent>();

            return movement.CanMove() ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}