using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class MoveToTargetEntity : BehaviourTreeLogicNode
    {
        private bool done;
        private bool failure;

        public MoveToTargetEntity(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override void OnOpen(BehaviourTreeContext context)
        {
            this.done = false;
            this.failure = false;

            var movement = context.Entity.GetComponent<MovementComponent>();

            if (!movement.CanMove() || context.TargetEntity == null)
            {
                this.failure = true;
                return;
            }

            movement.Stopped += OnMovementStopped;
            movement.Move(context.TargetEntity.transform.position);
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