using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsTargetEntityBehind : BehaviourTreeLogicNode
    {
        public IsTargetEntityBehind(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cell = BoardNavigator.Instance.BehindTheBackOfOccupying(context.Entity.transform.position);

            if (cell == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            return cell.OccupiedBy == context.RequireTargetEntity()
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}