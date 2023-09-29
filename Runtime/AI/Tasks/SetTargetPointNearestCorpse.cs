using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointNearestCorpse : BehaviourTreeLogicNode
    {
        public SetTargetPointNearestCorpse(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var cell = Board.Instance.Cells
                .OrderBy(c => (c.transform.position - context.Entity.transform.position).sqrMagnitude)
                .FirstOrDefault(c => c.GameObjectsInside.Corpses().Any());

            if (cell == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetPoint = cell.transform.position;
            return BehaviourTreeStatus.Success;
        }
    }
}