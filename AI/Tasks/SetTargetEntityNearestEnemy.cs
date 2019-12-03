using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityNearestEnemy : BehaviourTreeLogicNode
    {
        public SetTargetEntityNearestEnemy(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var closest = Scene.Active.Entities
                .Alive(entity => entity.IsEnemyOf(context.Entity) &&
                                 !entity.GetComponent<BehavioursComponent>().IsInvisible)
                .OrderBy(entity => (context.Entity.transform.position - entity.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closest == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetEntity = closest.gameObject;

            return BehaviourTreeStatus.Success;
        }
    }
}