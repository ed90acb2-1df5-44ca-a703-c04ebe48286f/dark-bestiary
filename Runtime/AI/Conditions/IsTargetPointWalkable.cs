using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetPointWalkable : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;

        public IsTargetPointWalkable(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cell = m_BoardNavigator.WithinCircle(context.RequireTargetPoint(), 1).FirstOrDefault();

            return cell == null || !cell.IsWalkable
                ? BehaviourTreeStatus.Failure
                : BehaviourTreeStatus.Success;
        }
    }
}