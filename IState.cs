namespace DarkBestiary
{
    public interface IState
    {
        void Enter();

        void Exit();

        void Tick(float delta);
    }
}