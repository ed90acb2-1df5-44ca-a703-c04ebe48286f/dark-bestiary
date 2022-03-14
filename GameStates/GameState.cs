using DarkBestiary.Messaging;

namespace DarkBestiary.GameStates
{
    public abstract class GameState : IState
    {
        public static event Payload<GameState> AnyGameStateEnter;
        public static event Payload<GameState> AnyGameStateExit;

        public bool IsMainMenu => this is MainMenuGameState;
        public bool IsCharacterCreation => this is CharacterCreationGameState;
        public bool IsCharacterSelection => this is CharacterSelectionGameState;
        public bool IsTown => this is TownGameState;
        public bool IsScenario => this is ScenarioGameState;
        public bool IsCredits => this is CreditsGameState;
        public bool IsVisionsMap => this is VisionMapGameState;
        public bool IsHub => this is TownGameState || this is VisionMapGameState;

        public void Enter()
        {
            OnEnter();

            AnyGameStateEnter?.Invoke(this);
        }

        public void Exit()
        {
            OnExit();

            AnyGameStateExit?.Invoke(this);
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