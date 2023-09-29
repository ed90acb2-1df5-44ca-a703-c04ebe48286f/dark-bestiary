using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TownViewController : ViewController<ITownView>
    {
        protected TownViewController(ITownView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.RemoveGemsRequested += () => Game.Instance.SwitchView<RemoveGemsViewController>();
            View.DismantlingRequested += () => Game.Instance.SwitchView<DismantlingViewController>();
            View.SocketingRequested += () => Game.Instance.SwitchView<SocketingViewController>();
            View.StashRequested += () => Game.Instance.SwitchView<StashViewController>();
            View.VendorRequested += () => Game.Instance.SwitchView<VendorViewController>();
            View.GambleRequested += () => Game.Instance.SwitchView<GambleViewController>();
            View.TransmutationRequested += () => Game.Instance.SwitchView<TransmutationViewController>();
            View.SphereCraftRequested += () => Game.Instance.SwitchView<SphereCraftViewController>();
            View.RuneCraftRequested += () => Game.Instance.SwitchView<RuneCraftViewController>();

            View.BestiaryRequested += () => Game.Instance.SwitchView<BestiaryViewController>();
            View.AlchemyRequested += () => Game.Instance.SwitchView<AlchemyViewController>();
            View.BlacksmithCraftRequested += () => Game.Instance.SwitchView<BlacksmithViewController>();
            View.EateryRequested += () => Game.Instance.SwitchView<EateryViewController>();

            View.MapRequested += () => Game.Instance.ToMap();

            View.ForgottenDepthsRequested += () => Game.Instance.SwitchView<ForgottenDepthsViewController>();
            View.RuneInscriptionRequested += () => Game.Instance.SwitchView<RuneInscriptionViewController>();
            View.LeaderboardRequested += () => Game.Instance.SwitchView<LeaderboardViewController>();
        }
    }
}