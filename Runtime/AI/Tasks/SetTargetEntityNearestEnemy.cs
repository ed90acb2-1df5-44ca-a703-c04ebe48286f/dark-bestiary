using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityNearestEnemy : BehaviourTreeLogicNode
    {
        public SetTargetEntityNearestEnemy(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var enemies = Scene.Active.Entities
                .Alive(entity => entity.IsEnemyOf(context.Entity) &&
                                 !entity.GetComponent<BehavioursComponent>().IsInvisible)
                .OrderBy(entity => (context.Entity.transform.position - entity.transform.position).sqrMagnitude)
                .ToList();

            var target = enemies.FirstOrDefault();

            if (enemies.Count > 1)
            {
                target = enemies.FirstOrDefault(e => !e.GetComponent<BehavioursComponent>().IsUncontrollableAndBreaksOnHit);

                if (target == null)
                {
                    target = enemies.FirstOrDefault();
                }
            }

            if (target == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetEntity = target;

            return BehaviourTreeStatus.Success;
        }
    }
}