using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.GameStates
{
    public class OutroGameState : GameState
    {
        private OutroViewController controller;

        protected override void OnEnter()
        {
            this.controller = ViewControllerRegistry.Initialize<OutroViewController>();
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