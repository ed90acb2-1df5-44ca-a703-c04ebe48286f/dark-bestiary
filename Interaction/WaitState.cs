using System;

namespace DarkBestiary.Interaction
{
    public class WaitState : InteractorState
    {
        private readonly Interactor interactor;

        private Func<bool> condition;

        public WaitState(Interactor interactor)
        {
            this.interactor = interactor;
        }

        public void Setup(Func<bool> condition)
        {
            this.condition = condition;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Tick(float delta)
        {
            if (!this.condition())
            {
                return;
            }

            this.interactor.EnterSelectionState();
        }
    }
}