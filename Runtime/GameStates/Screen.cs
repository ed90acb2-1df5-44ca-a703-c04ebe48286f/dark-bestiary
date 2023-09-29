namespace DarkBestiary.GameStates
{
    public abstract class Screen
    {
        public void Enter()
        {
            OnEnter();
        }

        public void Exit()
        {
            OnExit();
        }

        public void Tick(float delta)
        {
            OnTick(delta);
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();
        protected abstract void OnTick(float delta);
    }
}