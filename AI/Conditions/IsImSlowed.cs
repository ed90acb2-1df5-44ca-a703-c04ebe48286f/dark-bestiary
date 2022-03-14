using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class IsImSlowed : BehaviourTreeLogicNode
    {
        public IsImSlowed(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return context.Entity.GetComponent<BehavioursComponent>().IsSlowed
                ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Failure;
        }
    }
}