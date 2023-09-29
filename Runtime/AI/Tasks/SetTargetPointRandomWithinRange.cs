using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointRandomWithinRange : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;

        public SetTargetPointRandomWithinRange(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cell = m_BoardNavigator.WithinCircle(context.Entity.transform.position, Properties.Range).Random();

            if (cell == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetPoint = cell.transform.position;
            return BehaviourTreeStatus.Success;
        }
    }
}