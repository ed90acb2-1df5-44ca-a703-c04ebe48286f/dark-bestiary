using System;
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
        public static bool IsForgottenDepthsEnabled;

        public string Version { get; }
        public GameMode Mode { get; set; }
        public GameState State { get; private set; }
        public bool IsCampaign => Mode == GameMode.Campaign;
        public bool IsVisions => Mode == GameMode.Visions;

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

            SwitchState(new LogosGameState());
        }

        public void SwitchState(Func<GameState> constructor, bool showLoadingIndicator)
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
            UIManager.HideAllTooltips();

            ScreenFade.Instance.To(() =>
                {
                    State?.Exit();
                    State = constructor.Invoke();
                    State.Enter();
                    CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
                }, showLoadingIndicator);
        }

        public void SwitchState(GameState state)
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
            UIManager.HideAllTooltips();

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
            SwitchState(new MainMenuGameState());
        }

        public void ToHub()
        {
            if (IsVisions)
            {
                SwitchState(new VisionMapGameState());
            }
            else
            {
                SwitchState(new TownGameState());
            }
        }

        public void ToCredits()
        {
            SwitchState(new CreditsGameState());
        }

        public void ToIntro()
        {
            SwitchState(new IntroGameState());
        }

        public void ToVisionMenu()
        {
            SwitchState(new VisionMenuGameState());
        }

        public void ToVisionMap()
        {
            SwitchState(new VisionMapGameState());
        }

        public void ToVisionTalents()
        {
            SwitchState(new VisionTalentsGameState());
        }

        public void ToVisionIntro()
        {
            SwitchState(new VisionIntroGameState());
        }

        public void ToOutro()
        {
            SwitchState(new OutroGameState());
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