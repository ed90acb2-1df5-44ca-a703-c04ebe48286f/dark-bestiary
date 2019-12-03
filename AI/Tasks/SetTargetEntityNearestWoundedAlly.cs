using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityNearestWoundedAlly : BehaviourTreeLogicNode
    {
        private readonly float fraction;

        public SetTargetEntityNearestWoundedAlly(BehaviourTreePropertiesData properties) : base(properties)
        {
            this.fraction = properties.HealthFraction;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var closest = Scene.Active.Entities
                .Alive(entity => entity.IsAllyOf(context.Entity))
                .Where(entity => entity.GetComponent<HealthComponent>().HealthFraction <= this.fraction)
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