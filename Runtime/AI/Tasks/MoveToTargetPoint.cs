using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Tasks
{
    public class MoveToTargetPoint : BehaviourTreeLogicNode
    {
        private readonly IPathfinder m_Pathfinder;
        private bool m_Failure;
        private bool m_Done;

        public MoveToTargetPoint(BehaviourTreePropertiesData properties, IPathfinder pathfinder) : base(properties)
        {
            m_Pathfinder = pathfinder;
        }

        protected override void OnOpen(BehaviourTreeContext context)
        {
            m_Done = false;
            m_Failure = false;

            var movement = context.Entity.GetComponent<MovementComponent>();

            if (!movement.CanMove() || context.TargetPoint == null)
            {
                m_Failure = true;
                return;
            }

            movement.Stopped += OnMovementStopped;
            movement.Move(context.TargetPoint.Value, false);
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