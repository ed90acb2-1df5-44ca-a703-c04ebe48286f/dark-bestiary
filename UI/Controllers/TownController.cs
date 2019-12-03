using DarkBestiary.Messaging;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class TownController : IController
    {
        public static TownController Active { get; private set; }

        public event Payload<IView> ViewSwitched;

        public bool IsCraft => this.activeView == this.craftController.View;
        public bool IsRemoveGems => this.activeView == this.removeGemsController.View;
        public bool IsForging => this.activeView == this.itemForgingViewController.View;
        public bool IsSocketing => this.activeView == this.socketingController.View;
        public bool IsDismantling => this.activeView == this.dismantlingController.View;

        private readonly Town town;

        private CommandBoardViewController commandBoardController;
        private StashViewController stashController;
        private VendorViewController vendorController;
        private CraftViewController craftController;
        private SocketingViewController socketingController;
        private RemoveGemsViewController removeGemsController;
        private DismantlingViewController dismantlingController;
        private NavigationViewController navigationViewController;
        private SkillVendorViewController skillVendorViewController;
        private ItemForgingViewController itemForgingViewController;
        private BestiaryViewController bestiaryController;
        private GambleViewController gambleController;
        private ReliquaryViewController reliquaryController;
        private AlchemyViewController alchemyController;
        private EateryViewController eateryController;
        private IView activeView;

        public TownController(Town town)
        {
            this.town = town;
        }

        public void Initialize()
        {
            Active = this;

            this.navigationViewController = Container.Instance.Instantiate<NavigationViewController>();
            this.navigationViewController.Initialize();

            this.stashController = Container.Instance.Instantiate<StashViewController>();
            this.stashController.Initialize();
            this.stashController.View.Hide();

            this.eateryController = Container.Instance.Instantiate<EateryViewController>();
            this.eateryController.Initialize();
            this.eateryController.View.Hide();

            this.reliquaryController = Container.Instance.Instantiate<ReliquaryViewController>();
            this.reliquaryController.Initialize();
            this.reliquaryController.View.Hide();

            this.alchemyController = Container.Instance.Instantiate<AlchemyViewController>();
            this.alchemyController.Initialize();
            this.alchemyController.View.Hide();

            this.gambleController = Container.Instance.Instantiate<GambleViewController>();
            this.gambleController.Initialize();
            this.gambleController.View.Hide();

            this.bestiaryController = Container.Instance.Instantiate<BestiaryViewController>();
            this.bestiaryController.Initialize();
            this.bestiaryController.View.Hide();

            this.itemForgingViewController = Container.Instance.Instantiate<ItemForgingViewController>();
            this.itemForgingViewController.Initialize();
            this.itemForgingViewController.View.Hide();

            this.skillVendorViewController = Container.Instance.Instantiate<SkillVendorViewController>();
            this.skillVendorViewController.Initialize();
            this.skillVendorViewController.View.Hide();

            this.dismantlingController = Container.Instance.Instantiate<DismantlingViewController>();
            this.dismantlingController.Initialize();
            this.dismantlingController.View.Hide();

            this.removeGemsController = Container.Instance.Instantiate<RemoveGemsViewController>();
            this.removeGemsController.Initialize();
            this.removeGemsController.View.Hide();

            this.socketingController = Container.Instance.Instantiate<SocketingViewController>();
            this.socketingController.Initialize();
            this.socketingController.View.Hide();

            this.craftController = Container.Instance.Instantiate<CraftViewController>();
            this.craftController.Initialize();
            this.craftController.View.Hide();

            this.vendorController = Container.Instance.Instantiate<VendorViewController>();
            this.vendorController.Initialize();
            this.vendorController.View.Hide();

            this.commandBoardController = Container.Instance.Instantiate<CommandBoardViewController>();
            this.commandBoardController.Initialize();
            this.commandBoardController.View.Hide();

            this.town.SkillVendorRequested += () => SwitchView(this.skillVendorViewController.View);
            this.town.RemoveGemsRequested += () => SwitchView(this.removeGemsController.View);
            this.town.DismantlingRequested += () => SwitchView(this.dismantlingController.View);
            this.town.SocketingRequested += () => SwitchView(this.socketingController.View);
            this.town.BestiaryRequested += () => SwitchView(this.bestiaryController.View);
            this.town.StashRequested += () => SwitchView(this.stashController.View);
            this.town.CraftRequested += () => SwitchView(this.craftController.View);
            this.town.VendorRequested += () => SwitchView(this.vendorController.View);
            this.town.GambleRequested += () => SwitchView(this.gambleController.View);
            this.town.ScenariosRequested += () => SwitchView(this.commandBoardController.View);
            this.town.ForgingRequested += () => SwitchView(this.itemForgingViewController.View);
            this.town.ReliquaryRequested += () => SwitchView(this.reliquaryController.View);
            this.town.AlchemyRequested += () => SwitchView(this.alchemyController.View);
            this.town.EateryRequested += () => SwitchView(this.eateryController.View);
        }

        public void Terminate()
        {
            this.alchemyController.Terminate();
            this.reliquaryController.Terminate();
            this.removeGemsController.Terminate();
            this.itemForgingViewController.Terminate();
            this.skillVendorViewController.Terminate();
            this.navigationViewController.Terminate();
            this.stashController.Terminate();
            this.commandBoardController.Terminate();
            this.vendorController.Terminate();
            this.craftController.Terminate();
            this.socketingController.Terminate();
            this.dismantlingController.Terminate();
            this.bestiaryController.Terminate();
            this.gambleController.Terminate();
            this.eateryController.Terminate();

            Object.Destroy(this.town.gameObject);
        }

        public void ShowCraft()
        {
            SwitchView(this.craftController.View);
        }

        public void ShowForging()
        {
            SwitchView(this.itemForgingViewController.View);
        }

        public void ShowRemoveGems()
        {
            SwitchView(this.removeGemsController.View);
        }

        public void ShowSocketing()
        {
            SwitchView(this.socketingController.View);
        }

        public void ShowDismantling()
        {
            SwitchView(this.dismantlingController.View);
        }

        private void SwitchView(IView view)
        {
            this.activeView?.Hide();
            this.activeView = view;
            this.activeView.Show();

            ViewSwitched?.Invoke(this.activeView);
        }
    }
}