using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.GameStates
{
    public class TownGameState : GameState
    {
        public static event Payload<TownGameState> Entered;

        public event Payload<IView> ViewSwitched;

        public bool IsCraft => this.activeView == BlacksmithController.View;
        public bool IsRemoveGems => this.activeView == RemoveGemsController.View;
        public bool IsForging => this.activeView == ItemForgingViewController.View;
        public bool IsSocketing => this.activeView == SocketingController.View;
        public bool IsDismantling => this.activeView == DismantlingController.View;
        public bool IsVendor => this.activeView == VendorController.View;
        public bool IsGamble => this.activeView == GambleController.View;

        public bool IsAlchemy => this.activeView == AlchemyController.View;
        public bool IsBestiary => this.activeView == BestiaryController.View;
        public bool IsTransmutation => this.activeView == TransmutationController.View;
        public bool IsSphereCraft => this.activeView == SphereCraftController.View;

        public TownViewController TownController { get; private set; }
        public CommandBoardViewController CommandBoardController { get; private set; }
        public StashViewController StashController { get; private set; }
        public VendorViewController VendorController { get; private set; }
        public AlchemyViewController AlchemyController { get; private set; }
        public BlacksmithViewController BlacksmithController { get; private set; }
        public SocketingViewController SocketingController { get; private set; }
        public RemoveGemsViewController RemoveGemsController { get; private set; }
        public DismantlingViewController DismantlingController { get; private set; }
        public NavigationViewController NavigationViewController { get; private set; }
        public ItemForgingViewController ItemForgingViewController { get; private set; }
        public BestiaryViewController BestiaryController { get; private set; }
        public GambleViewController GambleController { get; private set; }
        public TransmutationViewController TransmutationController { get; private set; }
        public SphereCraftViewController SphereCraftController { get; private set; }
        public EateryViewController EateryController { get; private set; }
        public ForgottenDepthsViewController ForgottenDepthsController { get; private set; }
        public RunesViewController RunesController { get; private set; }
        public RuneforgeViewController RuneforgeController { get; private set; }
        public LeaderboardViewController LeaderboardViewController { get; private set; }
        public CharacterUnitFrameViewController CharacterUnitFrameController { get; private set; }

        private IView activeView;

        protected override void OnEnter()
        {
            TownController = ViewControllerRegistry.Initialize<TownViewController>(new object[] {this});
            TownController.View.Show();

            NavigationViewController = ViewControllerRegistry.Initialize<NavigationViewController>();
            NavigationViewController.View.Show();

            CharacterUnitFrameController = ViewControllerRegistry.Initialize<CharacterUnitFrameViewController>();
            CharacterUnitFrameController.View.Show();

            StashController = ViewControllerRegistry.Initialize<StashViewController>();
            TransmutationController = ViewControllerRegistry.Initialize<TransmutationViewController>();
            SphereCraftController = ViewControllerRegistry.Initialize<SphereCraftViewController>();
            GambleController = ViewControllerRegistry.Initialize<GambleViewController>();
            ItemForgingViewController = ViewControllerRegistry.Initialize<ItemForgingViewController>();
            DismantlingController = ViewControllerRegistry.Initialize<DismantlingViewController>();
            RemoveGemsController = ViewControllerRegistry.Initialize<RemoveGemsViewController>();
            SocketingController = ViewControllerRegistry.Initialize<SocketingViewController>();
            RunesController = ViewControllerRegistry.Initialize<RunesViewController>();
            RuneforgeController = ViewControllerRegistry.Initialize<RuneforgeViewController>();
            VendorController = ViewControllerRegistry.Initialize<VendorViewController>(new object[]
            {
                new List<VendorPanel.Category>
                {
                    VendorPanel.Category.All,
                    VendorPanel.Category.Armor,
                    VendorPanel.Category.Weapon,
                    VendorPanel.Category.Miscellaneous,
                    VendorPanel.Category.Buyout
                }
            });

            var equipmentView = ViewControllerRegistry.Get<EquipmentViewController>().View;
            equipmentView.Connect(StashController.View);
            equipmentView.Connect(TransmutationController.View);
            equipmentView.Connect(SphereCraftController.View);
            equipmentView.Connect(GambleController.View);
            equipmentView.Connect(ItemForgingViewController.View);
            equipmentView.Connect(DismantlingController.View);
            equipmentView.Connect(RemoveGemsController.View);
            equipmentView.Connect(SocketingController.View);
            equipmentView.Connect(VendorController.View);
            equipmentView.Connect(RunesController.View);

            EateryController = ViewControllerRegistry.Initialize<EateryViewController>();
            ForgottenDepthsController = ViewControllerRegistry.Initialize<ForgottenDepthsViewController>();
            LeaderboardViewController = ViewControllerRegistry.Initialize<LeaderboardViewController>();
            BestiaryController = ViewControllerRegistry.Initialize<BestiaryViewController>();
            AlchemyController = ViewControllerRegistry.Initialize<AlchemyViewController>();
            BlacksmithController = ViewControllerRegistry.Initialize<BlacksmithViewController>();
            CommandBoardController = ViewControllerRegistry.Initialize<CommandBoardViewController>();

            var character = CharacterManager.Instance.Character;

            if (!character.IsStartScenarioCompleted)
            {
                Game.Instance.SwitchState(
                    new ScenarioGameState(
                        Container.Instance.Resolve<IScenarioRepository>().Starting(),
                        Container.Instance.Resolve<CharacterManager>().Character
                    )
                );

                return;
            }

            // TODO: Experience recalculation do not trigger achievements
            // Note: this stuff is required to recalculate levels, useful when changing exp formulas
            character.Entity.GetComponent<ExperienceComponent>().Experience.Add(0);
            character.Entity.GetComponent<ReliquaryComponent>().Available.ForEach(r => r.Experience.Add(0));

            MusicManager.Instance.Play("event:/Music/Town");

            Entered?.Invoke(this);
        }

        protected override void OnExit()
        {
            ViewControllerRegistry.Get<EquipmentViewController>().View.DisconnectAll();

            TownController.Terminate();
            CharacterUnitFrameController.Terminate();
            NavigationViewController.Terminate();
            StashController.Terminate();
            EateryController.Terminate();
            TransmutationController.Terminate();
            SphereCraftController.Terminate();
            GambleController.Terminate();
            BestiaryController.Terminate();
            ItemForgingViewController.Terminate();
            DismantlingController.Terminate();
            RemoveGemsController.Terminate();
            SocketingController.Terminate();
            AlchemyController.Terminate();
            BlacksmithController.Terminate();
            VendorController.Terminate();
            CommandBoardController.Terminate();
            ForgottenDepthsController.Terminate();
            RunesController.Terminate();
            RuneforgeController.Terminate();
            LeaderboardViewController.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }

        public void SwitchView(IView view)
        {
            this.activeView?.Hide();
            this.activeView = view;
            this.activeView.Show();

            ViewSwitched?.Invoke(this.activeView);
        }
    }
}