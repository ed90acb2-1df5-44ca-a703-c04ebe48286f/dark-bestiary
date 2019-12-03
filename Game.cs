using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary
{
    public class Game : IStateMachine<GameState>
    {
        public static Game Instance { get; private set; }

        public string Version { get; }
        public GameState State { get; private set; }

        public Game(string version)
        {
            Version = version;
            Instance = this;
        }

        public void Start()
        {
            if (Debug.isDebugBuild)
            {
                Container.Instance.Instantiate<DeveloperConsoleViewController>().Initialize();
            }

            SwitchState(new EarlyAccessWarningGameState());
        }

        public void SwitchState(GameState state)
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);

            ScreenFade.Instance.To(() =>
            {
                State?.Exit();
                State = state;
                State.Enter();
                CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
            }, state.IsScenario || state.IsTown);
        }

        public void ToMainMenu()
        {
            Instance.SwitchState(new MainMenuGameState());
        }

        public void ToCharacterSelection()
        {
            Instance.SwitchState(new CharacterSelectionGameState());
        }

        public void ToTown()
        {
            Instance.SwitchState(new TownGameState());
        }

        public void ToCredits()
        {
            Instance.SwitchState(new CreditsGameState());
        }

        public void ToIntro()
        {
            Instance.SwitchState(new IntroGameState());
        }

        public void ToOutro()
        {
            Instance.SwitchState(new OutroGameState());
        }

        public void Tick(float delta)
        {
            State?.Tick(delta);
        }

        public static void Quit()
        {
            Application.Quit();
        }
    }
}