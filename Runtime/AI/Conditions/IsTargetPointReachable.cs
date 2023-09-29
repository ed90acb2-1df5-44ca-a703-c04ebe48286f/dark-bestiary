using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointReachable : BehaviourTreeLogicNode
    {
        private readonly IPathfinder m_Pathfinder;

        public IsTargetPointReachable(BehaviourTreePropertiesData properties, IPathfinder pathfinder) : base(properties)
        {
            m_Pathfinder = pathfinder;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return m_Pathfinder.IsTargetReachable(context.Entity, context.RequireTargetPoint())
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}