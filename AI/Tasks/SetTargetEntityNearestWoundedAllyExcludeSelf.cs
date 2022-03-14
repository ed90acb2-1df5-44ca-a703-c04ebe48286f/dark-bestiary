using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityNearestWoundedAllyExcludeSelf : BehaviourTreeLogicNode
    {
        private readonly float fraction;

        public SetTargetEntityNearestWoundedAllyExcludeSelf(BehaviourTreePropertiesData properties) : base(properties)
        {
            this.fraction = properties.HealthFraction;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var closest = Scene.Active.Entities
                .Alive(entity => entity != context.Entity &&
                                 entity.IsAllyOf(context.Entity) &&
                                !entity.GetComponent<UnitComponent>().IsUndead &&
                                 entity.GetComponent<HealthComponent>().HealthFraction <= this.fraction)
                .OrderBy(entity => (context.Entity.transform.position - entity.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (closest == null)
            {
                return BehaviourTreeStatus.Failure;
            }

            context.TargetEntity = closest;
            return BehaviourTreeStatus.Success;
        }
    }
}