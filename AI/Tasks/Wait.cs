using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class Wait : BehaviourTreeLogicNode
    {
        private readonly float duration;
        private float counter;

        public Wait(BehaviourTreePropertiesData properties) : base(properties)
        {
            this.duration = properties.WaitDuration;
        }

        protected override void OnClose(BehaviourTreeContext context)
        {
            this.counter = 0;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            this.counter += delta;

            return this.counter >= this.duration ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Running;
        }
    }
}