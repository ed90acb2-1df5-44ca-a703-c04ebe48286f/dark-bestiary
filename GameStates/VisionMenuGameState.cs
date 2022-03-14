using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class VisionMenuGameState : GameState
    {
        private VisionMenuViewController visionMenuViewController;

        protected override void OnEnter()
        {
            this.visionMenuViewController = Container.Instance.Instantiate<VisionMenuViewController>();
            this.visionMenuViewController.Initialize();
            this.visionMenuViewController.View.Show();
        }

        protected override void OnExit()
        {
            this.visionMenuViewController.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}