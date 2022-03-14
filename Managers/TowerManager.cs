using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.Managers
{
    public class TowerManager : Zenject.IInitializable
    {
        public const int MonsterLevelGrowthPerEpisode = 6;
        public const int MonsterLevelGrowthPerBoss = 20;
        public const int BossEpisodeNumber = 4;

        public static event Payload<int> FloorCompleted;

        private readonly List<Item> items = new List<Item>();
        private readonly CharacterManager characterManager;
        private readonly IItemRepository itemRepository;

        private BuffSelectionViewController buffSelectionViewController;
        private TowerConfirmationViewController towerConfirmationViewController;

        public TowerManager(CharacterManager characterManager, IItemRepository itemRepository)
        {
            this.characterManager = characterManager;
            this.itemRepository = itemRepository;
        }

        public void Initialize()
        {
            Episode.AnyEpisodeCompleted += OnEpisodeCompleted;
            Scenario.AnyScenarioStarted += OnScenarioStarted;
            Scenario.AnyScenarioCompleted += OnScenarioCompleted;
            Scenario.AnyScenarioFailed += OnScenarioFailed;
        }

        private void OnScenarioStarted(Scenario scenario)
        {
            if (!scenario.IsAscension)
            {
                return;
            }

            HealthComponent.AnyEntityDied += OnAnyEntityDied;
        }

        private void OnScenarioCompleted(Scenario scenario)
        {
            if (!scenario.IsAscension)
            {
                return;
            }

            GiveLootAndExperience();

            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
        }

        private void OnScenarioFailed(Scenario scenario)
        {
            if (!scenario.IsAscension)
            {
                return;
            }

            this.items.Clear();

            HealthComponent.AnyEntityDied -= OnAnyEntityDied;
        }

        private void OnAnyEntityDied(EntityDiedEventData payload)
        {
            if (!payload.Victim.IsCharacter())
            {
                return;
            }

            OnCharacterDied();
        }

        private void OnCharacterDied()
        {
            RemoveStuff();
        }

        private void OnEpisodeCompleted(Episode episode)
        {
            if (!episode.Scenario.IsAscension)
            {
                return;
            }

            var floor = (episode.Scenario.ActiveEpisodeIndex + 1) / BossEpisodeNumber;

            if ((episode.Scenario.ActiveEpisodeIndex + 1) % BossEpisodeNumber > 0)
            {
                this.buffSelectionViewController = ViewControllerRegistry.Initialize<BuffSelectionViewController>();
                this.buffSelectionViewController.Terminated += OnBuffSelectionViewControllerTerminated;
                this.buffSelectionViewController.View.Show();
                return;
            }

            if (episode.Scenario.IsActiveEpisodeLast)
            {
                return;
            }

            FloorCompleted?.Invoke(floor);
            AddItem(GetItemByTier(floor));

            this.towerConfirmationViewController = ViewControllerRegistry.Initialize<TowerConfirmationViewController>(new object[] {this.items});
            this.towerConfirmationViewController.Terminated += OnTowerConfirmationViewControllerTerminated;
            this.towerConfirmationViewController.View.Show();
        }

        private Item GetItemByTier(int tier)
        {
            int itemId;

            switch (tier)
            {
                case 1:
                    itemId = Constants.ItemIdTowerChestUniqueEquipment;
                    break;
                case 2:
                    itemId = Constants.ItemIdTowerChestLegendaryEquipment;
                    break;
                case 3:
                    itemId = Constants.ItemIdTowerChestLegendaryGems;
                    break;
                case 4:
                    itemId = Constants.ItemIdTowerChestLegendaryGems;
                    break;
                default:
                    itemId = Constants.ItemIdSphereOfAscension;
                    break;
            }

            return this.itemRepository.Find(itemId);
        }

        private void OnBuffSelectionViewControllerTerminated(IViewController<IBuffSelectionView> viewController)
        {
            this.buffSelectionViewController.Terminated -= OnBuffSelectionViewControllerTerminated;

            this.characterManager.Character.Entity.GetComponent<BehavioursComponent>().ApplyAllStacks(
                this.buffSelectionViewController.SelectedBuff, this.characterManager.Character.Entity);
        }

        private void OnTowerConfirmationViewControllerTerminated(IViewController<ITowerConfirmationView> viewController)
        {
            this.towerConfirmationViewController.Terminated -= OnTowerConfirmationViewControllerTerminated;

            if (this.towerConfirmationViewController.IsContinuing)
            {
                return;
            }

            GiveLootAndExperience();
            RemoveStuff();

            Game.Instance.ToHub();
        }

        private void RemoveStuff()
        {
            this.characterManager.Character.Data.AvailableScenarios.Remove(Scenario.Active.Id);
            this.characterManager.Character.Entity.GetComponent<BehavioursComponent>().RemoveBehaviours(b => b.IsAscension);
        }

        private void GiveLootAndExperience()
        {
            this.characterManager.Character.Entity.GetComponent<InventoryComponent>().Pickup(this.items);
            this.items.Clear();

            Scenario.Active.GiveExperience();
        }

        private void AddItem(Item item)
        {
            if (item.IsStackable)
            {
                var stackable = this.items.FirstOrDefault(i => i.Id == item.Id);

                if (stackable != null)
                {
                    stackable.AddStack(item.StackCount);
                    return;
                }
            }

            this.items.Add(item);
        }
    }
}