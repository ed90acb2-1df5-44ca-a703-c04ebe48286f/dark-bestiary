using System;

namespace DarkBestiary.Interaction
{
    public class WaitState : InteractorState
    {
        private readonly Interactor m_Interactor;

        private Func<bool> m_Condition;

        public WaitState(Interactor interactor)
        {
            m_Interactor = interactor;
        }

        public void Setup(Func<bool> condition)
        {
            m_Condition = condition;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Tick(float delta)
        {
            if (!m_Condition())
            {
                return;
            }

            m_Interactor.EnterSelectionState();
        }
    }
}