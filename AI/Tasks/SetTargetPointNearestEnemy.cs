using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetPointNearestEnemy : BehaviourTreeLogicNode
    {
        public SetTargetPointNearestEnemy(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var closestEnemy = Scene.Active.Entities
                .Alive(entity => entity.IsEnemyOf(context.Entity))
                .OrderBy(entity => (context.Entity.transform.position - entity.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closestEnemy == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetPoint = closestEnemy.transform.position;

            return BehaviourTreeStatus.Success;
        }
    }
}