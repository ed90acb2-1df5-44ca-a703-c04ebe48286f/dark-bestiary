using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityReachable : BehaviourTreeLogicNode
    {
        private readonly IPathfinder m_Pathfinder;

        public IsTargetEntityReachable(BehaviourTreePropertiesData properties,
            IPathfinder pathfinder) : base(properties)
        {
            m_Pathfinder = pathfinder;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return m_Pathfinder.IsTargetReachable(context.Entity, context.RequireTargetEntity().transform.position)
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}