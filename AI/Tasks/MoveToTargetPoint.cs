using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Tasks
{
    public class MoveToTargetPoint : BehaviourTreeLogicNode
    {
        private readonly IPathfinder pathfinder;
        private bool failure;
        private bool done;

        public MoveToTargetPoint(BehaviourTreePropertiesData properties, IPathfinder pathfinder) : base(properties)
        {
            this.pathfinder = pathfinder;
        }

        protected override void OnOpen(BehaviourTreeContext context)
        {
            this.done = false;
            this.failure = false;

            var movement = context.Entity.GetComponent<MovementComponent>();

            if (!movement.CanMove() || context.TargetPoint == null)
            {
                this.failure = true;
                return;
            }

            movement.Stopped += OnMovementStopped;
            movement.Move(context.TargetPoint.Value, false);
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (this.failure)
            {
                return BehaviourTreeStatus.Failure;
            }

            return this.done ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Running;
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            this.done = true;
        }
    }
}