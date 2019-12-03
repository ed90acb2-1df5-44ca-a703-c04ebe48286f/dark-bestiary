using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class OutroGameState : GameState
    {
        private OutroViewController controller;

        protected override void OnEnter()
        {
            this.controller = Container.Instance.Instantiate<OutroViewController>();
            this.controller.Initialize();
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