namespace DarkBestiary.UI.Controllers
{
    public class HudController : IController
    {
        private ActionBarViewController actionBarViewController;
        private TargetFrameViewController targetFrameViewController;

        public void Initialize()
        {
            this.actionBarViewController = Container.Instance.Instantiate<ActionBarViewController>();
            this.actionBarViewController.Initialize();

            this.targetFrameViewController = Container.Instance.Instantiate<TargetFrameViewController>();
            this.targetFrameViewController.Initialize();
        }

        public void Terminate()
        {
            this.actionBarViewController.Terminate();
            this.targetFrameViewController.Terminate();
        }
    }
}