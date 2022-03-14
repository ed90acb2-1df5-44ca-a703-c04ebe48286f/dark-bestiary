using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class VisionTalentsGameState : GameState
    {
        private VisionTalentsViewController controller;

        protected override void OnEnter()
        {
            this.controller = ViewControllerRegistry.Initialize<VisionTalentsViewController>();
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