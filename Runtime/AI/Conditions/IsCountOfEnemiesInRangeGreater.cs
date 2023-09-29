using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class IsCountOfEnemiesInRangeGreater : BehaviourTreeLogicNode
    {
        public IsCountOfEnemiesInRangeGreater(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var enemies = BoardNavigator.Instance
                .WithinCircle(context.Entity.transform.position, Properties.Range)
                .Count(c => c.OccupiedBy?.IsEnemyOf(context.Entity) == true);

            return enemies > Properties.Count
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}