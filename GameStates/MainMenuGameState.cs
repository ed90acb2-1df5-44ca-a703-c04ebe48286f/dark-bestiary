using DarkBestiary.Managers;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class MainMenuGameState : GameState
    {
        private MainMenuViewController controller;

        protected override void OnEnter()
        {
            this.controller = ViewControllerRegistry.Initialize<MainMenuViewController>();
            this.controller.View.Show();

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