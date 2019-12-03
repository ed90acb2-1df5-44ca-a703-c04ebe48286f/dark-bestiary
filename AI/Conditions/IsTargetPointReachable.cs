using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointReachable : BehaviourTreeLogicNode
    {
        private readonly IPathfinder pathfinder;

        public IsTargetPointReachable(BehaviourTreePropertiesData properties, IPathfinder pathfinder) : base(properties)
        {
            this.pathfinder = pathfinder;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return this.pathfinder.IsTargetReachable(context.Entity, context.RequireTargetPoint())
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}