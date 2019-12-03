using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityInRange : BehaviourTreeLogicNode
    {
        private readonly int range;
        private readonly BoardNavigator boardNavigator;

        public IsTargetEntityInRange(BoardNavigator boardNavigator, BehaviourTreePropertiesData properties) : base(properties)
        {
            this.boardNavigator = boardNavigator;
            this.range = properties.Range;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var target = context.TargetEntity;
            var entity = context.Entity;

            if (target == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return this.boardNavigator.EntitiesInRadius(entity.transform.position, this.range).Contains(target)
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}