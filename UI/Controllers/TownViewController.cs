using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TownViewController : ViewController<ITownView>
    {
        private readonly TownGameState gameState;

        protected TownViewController(ITownView view, TownGameState gameState) : base(view)
        {
            this.gameState = gameState;
        }

        protected override void OnInitialize()
        {
            View.RemoveGemsRequested += () => this.gameState.SwitchView(this.gameState.RemoveGemsController.View);
            View.DismantlingRequested += () => this.gameState.SwitchView(this.gameState.DismantlingController.View);
            View.SocketingRequested += () => this.gameState.SwitchView(this.gameState.SocketingController.View);
            View.StashRequested += () => this.gameState.SwitchView(this.gameState.StashController.View);
            View.VendorRequested += () => this.gameState.SwitchView(this.gameState.VendorController.View);
            View.GambleRequested += () => this.gameState.SwitchView(this.gameState.GambleController.View);
            View.ForgingRequested += () => this.gameState.SwitchView(this.gameState.ItemForgingViewController.View);
            View.TransmutationRequested += () => this.gameState.SwitchView(this.gameState.TransmutationController.View);
            View.SphereCraftRequested += () => this.gameState.SwitchView(this.gameState.SphereCraftController.View);
            View.RuneforgeRequested += () => this.gameState.SwitchView(this.gameState.RuneforgeController.View);

            View.BestiaryRequested += () => this.gameState.SwitchView(this.gameState.BestiaryController.View);
            View.AlchemyRequested += () => this.gameState.SwitchView(this.gameState.AlchemyController.View);
            View.BlacksmithCraftRequested += () => this.gameState.SwitchView(this.gameState.BlacksmithController.View);
            View.ScenariosRequested += () => this.gameState.SwitchView(this.gameState.CommandBoardController.View);
            View.EateryRequested += () => this.gameState.SwitchView(this.gameState.EateryController.View);

            View.ForgottenDepthsRequested += () => this.gameState.SwitchView(this.gameState.ForgottenDepthsController.View);
            View.RunesRequested += () => this.gameState.SwitchView(this.gameState.RunesController.View);
            View.LeaderboardRequested += () => this.gameState.SwitchView(this.gameState.LeaderboardViewController.View);
        }
    }
}