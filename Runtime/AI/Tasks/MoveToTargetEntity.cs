using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class MoveToTargetEntity : BehaviourTreeLogicNode
    {
        private bool m_Done;
        private bool m_Failure;

        public MoveToTargetEntity(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override void OnOpen(BehaviourTreeContext context)
        {
            m_Done = false;
            m_Failure = false;

            var movement = context.Entity.GetComponent<MovementComponent>();

            if (!movement.CanMove() || context.TargetEntity == null)
            {
                m_Failure = true;
                return;
            }

            movement.Stopped += OnMovementStopped;
            movement.Move(context.TargetEntity.transform.position);
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            if (m_Failure)
            {
                return BehaviourTreeStatus.Failure;
            }

            return m_Done ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Running;
        }

        private void OnMovementStopped(MovementComponent movement)
        {
            m_Done = true;
        }
    }
}