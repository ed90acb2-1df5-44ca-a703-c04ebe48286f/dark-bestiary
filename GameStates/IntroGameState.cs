using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class IntroGameState : GameState
    {
        private IntroViewController controller;

        protected override void OnEnter()
        {
            this.controller = ViewControllerRegistry.Initialize<IntroViewController>();
            this.controller.View.Show();
        }

        protected override void OnExit()
        {
            this.controller.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}