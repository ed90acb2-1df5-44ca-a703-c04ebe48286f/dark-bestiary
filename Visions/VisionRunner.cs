using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DarkBestiary.Visions
{
    public class VisionRunner
    {
        public static event Payload<VisionView> AnyCompleted;
        public event Payload<VisionView> Completed;
        public event Payload<VisionView> Failed;

        public bool IsRunning => this.running != null;

        private const int VisionStrengthBehaviourId = 861;

        private readonly IScenarioRepository scenarioRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly ILootDataRepository lootRepository;
        private readonly IItemRepository itemRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly IItemCategoryRepository itemCategoryRepository;
        private readonly VisionManager visionManager;
        private readonly CharacterManager characterManager;

        private ItemForgingViewController forgingController;
        private VendorViewController vendorController;

        private BuffSelectionViewController buffSelectionController;
        private EateryViewController eateryController;
        private GambleViewController gambleController;
        private VisionView running;

        public VisionRunner(
            VisionManager visionManager,
            CharacterManager characterManager,
            IScenarioRepository scenarioRepository,
            ICurrencyRepository currencyRepository,
            ILootDataRepository lootRepository,
            IItemRepository itemRepository,
            IBehaviourRepository behaviourRepository,
            IItemCategoryRepository itemCategoryRepository)
        {
            this.visionManager = visionManager;
            this.characterManager = characterManager;
            this.scenarioRepository = scenarioRepository;
            this.currencyRepository = currencyRepository;
            this.lootRepository = lootRepository;
            this.itemRepository = itemRepository;
            this.behaviourRepository = behaviourRepository;
            this.itemCategoryRepository = itemCategoryRepository;
        }

        public void Initialize()
        {
            this.vendorController = Container.Instance.Instantiate<VendorViewController>(new object[]{new List<VendorPanel.Category> {VendorPanel.Category.All}});
            this.vendorController.Initialize();
            this.vendorController.View.Hidden += OnVendorViewHidden;
            this.vendorController.View.RequiresConfirmationOnClose = true;

            this.forgingController = Container.Instance.Instantiate<ItemForgingViewController>();
            this.forgingController.Initialize();
            this.forgingController.View.RequiresConfirmationOnClose = true;
            this.forgingController.View.Hidden += OnForgingViewHidden;

            Scenario.AnyScenarioCompleted += OnAnyScenarioCompleted;
            Scenario.AnyScenarioFailed += OnAnyScenarioFailed;
        }

        public void Terminate()
        {
            this.vendorController.View.Hidden -= OnVendorViewHidden;
            this.vendorController.Terminate();

            Scenario.AnyScenarioCompleted -= OnAnyScenarioCompleted;
            Scenario.AnyScenarioFailed -= OnAnyScenarioFailed;
        }

        public void Run(VisionView vision)
        {
            this.running = vision;

            switch (this.running.Data.Type)
            {
                case VisionType.Vendor:
                    RunVendor(vision);
                    break;
                case VisionType.Scenario:
                    RunScenario(vision);
                    break;
                case VisionType.Gamble:
                    RunGamble(vision);
                    break;
                case VisionType.Eatery:
                    RunEatery(vision);
                    break;
                case VisionType.Forging:
                    RunForging(vision);
                    break;
                case VisionType.Buff:
                    RunBuff(vision);
                    break;
                case VisionType.Loot:
                    RunLoot(vision);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown vision type: {vision.Data.Type}");
            }
        }

        private void RunLoot(VisionView vision)
        {
            var item = new Item(0, I18N.Instance.Get("item_treasure_chest_name"), new ItemData {Icon = "Sprites/Icons/Items/icon_reward_chest_02"}, null);
            var loot = new Loot(this.lootRepository.Find(vision.Data.LootId), this.itemRepository, this.lootRepository, this.itemCategoryRepository);

            ContainerWindow.Instance.Show(item, loot, this.characterManager.Character.Entity);
            ContainerWindow.Instance.Hidden += OnContainerWindowHidden;
        }

        private void OnContainerWindowHidden()
        {
            ContainerWindow.Instance.Hidden -= OnContainerWindowHidden;
            OnVisionCompleted(this.running);
        }

        private void RunBuff(VisionView vision)
        {
            this.buffSelectionController = Container.Instance.Instantiate<BuffSelectionViewController>();
            this.buffSelectionController.Initialize();
            this.buffSelectionController.View.Hidden += OnBuffSelectionViewHidden;
            this.buffSelectionController.View.Show();
        }

        private void RunForging(VisionView vision)
        {
            this.forgingController.Reset();
            this.forgingController.View.Show();

            var equipmentViewController = ViewControllerRegistry.Get<EquipmentViewController>();
            equipmentViewController.View.Connect(this.forgingController.View);
            equipmentViewController.View.RequiresConfirmationOnClose = true;
            equipmentViewController.View.Show();
        }

        private void RunVendor(VisionView vision)
        {
            new Loot(
                this.lootRepository.Find(vision.Data.LootId),
                this.itemRepository,
                this.lootRepository,
                this.itemCategoryRepository)
                .RollDrop(this.visionManager.CurrentAct * 10, assortment =>
                {
                    foreach (var item in assortment)
                    {
                        if (!item.IsSkillBook)
                        {
                            continue;
                        }

                        var playerVisionFragments = this.characterManager.Character.Entity.GetComponent<CurrenciesComponent>().Get(CurrencyType.VisionFragment).Amount;
                        var cost = this.currencyRepository.FindByType(CurrencyType.VisionFragment).Set(Mathf.Clamp(playerVisionFragments, 1, 5));

                        item.ChangePrice(new List<Currency> {cost});
                        item.IsFixedPrice = true;
                    }

                    this.vendorController.ClearBuyback();
                    this.vendorController.ChangeAssortment(assortment);
                    this.vendorController.View.Show();

                    var equipmentViewController = ViewControllerRegistry.Get<EquipmentViewController>();
                    equipmentViewController.View.Connect(this.vendorController.View);
                    equipmentViewController.View.RequiresConfirmationOnClose = true;
                    equipmentViewController.View.Show();
                });
        }

        private void RunGamble(VisionView vision)
        {
            this.gambleController = Container.Instance.Instantiate<GambleViewController>();
            this.gambleController.Initialize();
            this.gambleController.View.RequiresConfirmationOnClose = true;
            this.gambleController.View.Hidden += OnGambleViewHidden;
            this.gambleController.View.Show();

            var equipmentViewController = ViewControllerRegistry.Get<EquipmentViewController>();
            equipmentViewController.View.Connect(this.gambleController.View);
            equipmentViewController.View.RequiresConfirmationOnClose = true;
            equipmentViewController.View.Show();
        }

        private void RunEatery(VisionView vision)
        {
            this.eateryController = Container.Instance.Instantiate<EateryViewController>();
            this.eateryController.Initialize();
            this.eateryController.View.RequiresConfirmationOnClose = true;
            this.eateryController.View.Hidden += OnEateryViewHidden;
            this.eateryController.View.Show();
        }

        private void RunScenario(VisionView vision)
        {
            ScreenFade.Instance.To(() =>
            {
                Game.Instance.SwitchState(() =>
                    {
                        var scenario = this.scenarioRepository.Find(vision.Data.ScenarioId);

                        AdjustScenarioMonsterLevel(vision, scenario);

                        return new ScenarioGameState(scenario, this.characterManager.Character);
                    }, true
                );
            });
        }

        private void AdjustScenarioMonsterLevel(VisionView vision, Scenario scenario)
        {
            foreach (var episode in scenario.Episodes)
            {
                foreach (var entity in episode.Scene.Entities.All().Where(entity => entity.IsEnemyOfPlayer()))
                {
                    var monsterLevel = VisionManager.Instance.GetVisionMonsterLevel(vision.Data, vision.ActIndex);

                    if (!vision.Data.IsFinal)
                    {
                        monsterLevel += RNG.Range(-1, 1);
                    }

                    entity.GetComponent<UnitComponent>().Level = Mathf.Max(1, monsterLevel);
                    entity.GetComponent<HealthComponent>().Restore();

                    var behaviour = this.behaviourRepository.Find(VisionStrengthBehaviourId);
                    behaviour.StackCount = this.visionManager.GetVisionStrength();
                    entity.GetComponent<BehavioursComponent>().ApplyAllStacks(behaviour, entity);
                }
            }
        }

        private void OnBuffSelectionViewHidden()
        {
            var behavioursComponent = this.characterManager.Character.Entity.GetComponent<BehavioursComponent>();
            behavioursComponent.ApplyAllStacks(this.buffSelectionController.SelectedBuff, behavioursComponent.gameObject);

            this.buffSelectionController.View.Hidden -= OnBuffSelectionViewHidden;

            OnVisionCompleted(this.running);
        }

        private void OnVendorViewHidden()
        {
            ViewControllerRegistry.Get<EquipmentViewController>().View.RequiresConfirmationOnClose = false;
            ViewControllerRegistry.Get<EquipmentViewController>().View.DisconnectAll();
            OnVisionCompleted(this.running);
        }

        private void OnForgingViewHidden()
        {
            ViewControllerRegistry.Get<EquipmentViewController>().View.RequiresConfirmationOnClose = false;
            ViewControllerRegistry.Get<EquipmentViewController>().View.DisconnectAll();
            OnVisionCompleted(this.running);
        }

        private void OnGambleViewHidden()
        {
            this.gambleController.View.Hidden -= OnGambleViewHidden;
            this.gambleController.Terminate();
            this.gambleController = null;

            ViewControllerRegistry.Get<EquipmentViewController>().View.RequiresConfirmationOnClose = false;
            ViewControllerRegistry.Get<EquipmentViewController>().View.DisconnectAll();
            OnVisionCompleted(this.running);
        }

        private void OnEateryViewHidden()
        {
            this.eateryController.View.Hidden -= OnEateryViewHidden;
            this.eateryController.Terminate();
            this.eateryController = null;

            OnVisionCompleted(this.running);
        }

        private void OnAnyScenarioFailed(Scenario scenario)
        {
            OnVisionFailed(this.running);
        }

        private void OnAnyScenarioCompleted(Scenario scenario)
        {
            OnVisionCompleted(this.running);
        }

        private void OnVisionCompleted(VisionView vision)
        {
            Completed?.Invoke(this.running);
            AnyCompleted?.Invoke(this.running);
            this.running = null;
        }

        private void OnVisionFailed(VisionView vision)
        {
            Failed?.Invoke(this.running);
            this.running = null;
        }
    }
}