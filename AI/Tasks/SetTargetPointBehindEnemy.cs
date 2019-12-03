using DarkBestiary.Data;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointBehindEnemy : BehaviourTreeLogicNode
    {
        private readonly BoardNavigator boardNavigator;

        public SetTargetPointBehindEnemy(BehaviourTreePropertiesData properties,
            BoardNavigator boardNavigator) : base(properties)
        {
            this.boardNavigator = boardNavigator;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return this.boardNavigator
                       .BehindTheBackOfOccupying(context.RequireTargetEntity().transform.position) == null
                ? BehaviourTreeStatus.Failure : BehaviourTreeStatus.Success;
        }
    }
}