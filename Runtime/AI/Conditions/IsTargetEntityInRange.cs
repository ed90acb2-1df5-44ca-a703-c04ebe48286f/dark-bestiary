using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityInRange : BehaviourTreeLogicNode
    {
        private readonly int m_Range;
        private readonly BoardNavigator m_BoardNavigator;

        public IsTargetEntityInRange(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            m_BoardNavigator = boardNavigator;
            m_Range = properties.Range;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var target = context.TargetEntity;
            var entity = context.Entity;

            if (target == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return m_BoardNavigator.EntitiesInRadius(entity.transform.position, m_Range).Contains(target)
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}