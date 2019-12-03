using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class IntroGameState : GameState
    {
        private IntroViewController controller;

        protected override void OnEnter()
        {
            this.controller = Container.Instance.Instantiate<IntroViewController>();
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