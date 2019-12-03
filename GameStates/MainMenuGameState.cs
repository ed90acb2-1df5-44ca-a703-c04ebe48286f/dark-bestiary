using DarkBestiary.Managers;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class MainMenuGameState : GameState
    {
        private MainMenuViewController controller;

        protected override void OnEnter()
        {
            this.controller = Container.Instance.Instantiate<MainMenuViewController>();
            this.controller.Initialize();

            MusicManager.Instance.Play("event:/Music/MainMenu");
        }

        protected override void OnTick(float delta)
        {
        }

        protected override void OnExit()
        {
            this.controller.Terminate();
        }
    }
}