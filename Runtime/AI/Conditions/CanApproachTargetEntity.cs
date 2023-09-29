using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Conditions
{
    public class CanApproachTargetEntity : BehaviourTreeLogicNode
    {
        private readonly IPathfinder m_Pathfinder;

        private bool? m_Success;

        public CanApproachTargetEntity(BehaviourTreePropertiesData properties,
            IPathfinder pathfinder) : base(properties)
        {
            m_Pathfinder = pathfinder;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var path = m_Pathfinder.FindPath(
                context.Entity, context.RequireTargetEntity().transform.position, true);

            return path.Count > 0 ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}