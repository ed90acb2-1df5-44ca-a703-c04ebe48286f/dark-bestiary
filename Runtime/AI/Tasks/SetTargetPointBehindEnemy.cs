using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointBehindEnemy : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator m_BoardNavigator;

        public SetTargetPointBehindEnemy(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return m_BoardNavigator
                       .BehindTheBackOfOccupying(context.RequireTargetEntity().transform.position) == null
                ? BehaviourTreeStatus.Failure : BehaviourTreeStatus.Success;
        }
    }
}