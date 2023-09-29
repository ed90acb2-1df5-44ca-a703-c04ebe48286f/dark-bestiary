using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;

namespace DarkBestiary.AI.Conditions
{
    public class AnyCorpsesInRange : BehaviourTreeLogicNode
    {
        public AnyCorpsesInRange(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return BoardNavigator.Instance
                .WithinCircle(context.Entity.transform.position, Properties.Range)
                .Any(cell => cell.GameObjectsInside.Corpses().Any())
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}