using DarkBestiary.Data;

namespace DarkBestiary.AI.Conditions
{
    public class Chance : BehaviourTreeLogicNode
    {
        public Chance(BehaviourTreePropertiesData properties) : base(properties)
        {
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            return RNG.Test(Properties.Chance)
                ? BehaviourTreeStatus.Success
                : BehaviourTreeStatus.Failure;
        }
    }
}