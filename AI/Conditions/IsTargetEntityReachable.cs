using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityReachable : BehaviourTreeLogicNode
    {
        private readonly IPathfinder pathfinder;

        public IsTargetEntityReachable(BehaviourTreePropertiesData properties,
            IPathfinder pathfinder) : base(properties)
        {
            this.pathfinder = pathfinder;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return this.pathfinder.IsTargetReachable(context.Entity, context.RequireTargetEntity().transform.position)
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}