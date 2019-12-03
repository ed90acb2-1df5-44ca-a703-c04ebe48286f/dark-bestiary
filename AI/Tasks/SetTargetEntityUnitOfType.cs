using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.AI.Tasks
{
    public class SetTargetEntityUnitOfType : BehaviourTreeLogicNode
    {
        public SetTargetEntityUnitOfType(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            var closest = Scene.Active.Entities
                .Alive(entity => entity.GetComponent<UnitComponent>().Id == Properties.UnitId &&
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