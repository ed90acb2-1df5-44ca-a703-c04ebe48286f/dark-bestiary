namespace DarkBestiary
{
    public interface IStateMachine<in TState> where TState : IState
    {
        void SwitchState(TState state);
    }
}