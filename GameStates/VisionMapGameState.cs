using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Views.Unity;
using DarkBestiary.Visions;

namespace DarkBestiary.GameStates
{
    public class VisionMapGameState : GameState
    {
        private NavigationViewController navigationController;

        protected override void OnEnter()
        {
            this.navigationController = ViewControllerRegistry.Initialize<NavigationViewController>();
            this.navigationController.View.Show();
            ((View) this.navigationController.View).transform.SetAsFirstSibling();

            VisionManager.MaybeInitialize();
        }

        protected override void OnExit()
        {
            this.navigationController.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}