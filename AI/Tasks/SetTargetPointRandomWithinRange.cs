using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointRandomWithinRange : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;

        public SetTargetPointRandomWithinRange(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            this.boardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cell = this.boardNavigator.WithinCircle(context.Entity.transform.position, Properties.Range).Random();

            if (cell == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetPoint = cell.transform.position;
            return BehaviourTreeStatus.Success;
        }
    }
}