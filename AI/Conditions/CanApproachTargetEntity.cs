using DarkBestiary.Data;
using DarkBestiary.Pathfinding;

namespace DarkBestiary.AI.Conditions
{
    public class CanApproachTargetEntity : BehaviourTreeLogicNode
    {
        private readonly IPathfinder pathfinder;

        private bool? success;

        public CanApproachTargetEntity(BehaviourTreePropertiesData properties,
            IPathfinder pathfinder) : base(properties)
        {
            this.pathfinder = pathfinder;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var path = this.pathfinder.FindPath(
                context.Entity, context.RequireTargetEntity().transform.position, true);

            return path.Count > 0 ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}