using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class VisionIntroGameState : GameState
    {
        private VisionIntroViewController introViewController;

        protected override void OnEnter()
        {
            this.introViewController = Container.Instance.Instantiate<VisionIntroViewController>();
            this.introViewController.View.Show();
            this.introViewController.Initialize();
        }

        protected override void OnExit()
        {
            this.introViewController.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}