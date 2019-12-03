using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointWalkable : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;

        public IsTargetPointWalkable(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            this.boardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cell = this.boardNavigator.WithinCircle(context.RequireTargetPoint(), 1).FirstOrDefault();

            return cell == null || !cell.IsWalkable
                ? BehaviourTreeStatus.Failure
                : BehaviourTreeStatus.Success;
        }
    }
}