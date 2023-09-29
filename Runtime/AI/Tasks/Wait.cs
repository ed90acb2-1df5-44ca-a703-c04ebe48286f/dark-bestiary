using DarkBestiary.Data;

namespace DarkBestiary.AI.Tasks
{
    public class Wait : BehaviourTreeLogicNode
    {
        private readonly float m_Duration;
        private float m_Counter;

        public Wait(BehaviourTreePropertiesData properties) : base(properties)
        {
            m_Duration = properties.WaitDuration;
        }

        protected override void OnClose(BehaviourTreeContext context)
        {
            m_Counter = 0;
        }

        protected override BehaviourTreeStatus OnTick(BehaviourTreeContext context, float delta)
        {
            m_Counter += delta;

            return m_Counter >= m_Duration ? BehaviourTreeStatus.Success : BehaviourTreeStatus.Running;
        }
    }
}