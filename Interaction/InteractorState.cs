namespace DarkBestiary.Interaction
{
    public abstract class InteractorState : IState
    {
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Tick(float delta);
    }
}