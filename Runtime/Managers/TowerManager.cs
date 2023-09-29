using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.Managers
{
    public class TowerManager : Zenject.IInitializable
    {
        public const int c_MonsterLevelGrowthPerEpisode = 6;
        public const int c_MonsterLevelGrowthPerBoss = 20;
        public const int c_BossEpisodeNumber = 4;

        public static event Action<int> FloorCompleted;

        private readonly List<Item> m_Items = new();
        private readonly IItemRepository m_ItemRepository;

        private BuffSelectionViewController m_BuffSelectionViewController;
        private TowerConfirmationViewController m_TowerConfirmationViewController;

        public TowerManager(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
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

            m_Items.Clear();

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

            var floor = (episode.Scenario.ActiveEpisodeIndex + 1) / c_BossEpisodeNumber;

            if ((episode.Scenario.ActiveEpisodeIndex + 1) % c_BossEpisodeNumber > 0)
            {
                m_BuffSelectionViewController = Container.Instance.Instantiate<BuffSelectionViewController>();
                m_BuffSelectionViewController.Initialize();
                m_BuffSelectionViewController.Terminated += OnBuffSelectionViewControllerTerminated;
                m_BuffSelectionViewController.View.Show();
                return;
            }

            if (episode.Scenario.IsActiveEpisodeLast)
            {
                return;
            }

            FloorCompleted?.Invoke(floor);
            AddItem(GetItemByTier(floor));

            m_TowerConfirmationViewController = Container.Instance.Instantiate<TowerConfirmationViewController>(new object[] { m_Items });
            m_TowerConfirmationViewController.Initialize();
            m_TowerConfirmationViewController.Terminated += OnTowerConfirmationViewControllerTerminated;
            m_TowerConfirmationViewController.View.Show();
        }

        private Item GetItemByTier(int tier)
        {
            int itemId;

            switch (tier)
            {
                case 1:
                    itemId = Constants.c_ItemIdTowerChestUniqueEquipment;
                    break;
                case 2:
                    itemId = Constants.c_ItemIdTowerChestLegendaryEquipment;
                    break;
                case 3:
                    itemId = Constants.c_ItemIdTowerChestLegendaryGems;
                    break;
                case 4:
                    itemId = Constants.c_ItemIdTowerChestLegendaryGems;
                    break;
                default:
                    itemId = Constants.c_ItemIdSphereOfAscension;
                    break;
            }

            return m_ItemRepository.Find(itemId);
        }

        private void OnBuffSelectionViewControllerTerminated(ViewController<IBuffSelectionView> viewController)
        {
            m_BuffSelectionViewController.Terminated -= OnBuffSelectionViewControllerTerminated;

            Game.Instance.Character.Entity.GetComponent<BehavioursComponent>().ApplyAllStacks(
                m_BuffSelectionViewController.SelectedBuff, Game.Instance.Character.Entity);
        }

        private void OnTowerConfirmationViewControllerTerminated(ViewController<ITowerConfirmationView> viewController)
        {
            m_TowerConfirmationViewController.Terminated -= OnTowerConfirmationViewControllerTerminated;

            if (m_TowerConfirmationViewController.IsContinuing)
            {
                return;
            }

            GiveLootAndExperience();
            RemoveStuff();

            Game.Instance.ToTown();
        }

        private void RemoveStuff()
        {
            Game.Instance.Character.Entity.GetComponent<BehavioursComponent>().RemoveBehaviours(b => b.IsAscension);
        }

        private void GiveLootAndExperience()
        {
            Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(m_Items);
            m_Items.Clear();

            Scenario.Active.GiveExperience();
        }

        private void AddItem(Item item)
        {
            if (item.IsStackable)
            {
                var stackable = m_Items.FirstOrDefault(i => i.Id == item.Id);

                if (stackable != null)
                {
                    stackable.AddStack(item.StackCount);
                    return;
                }
            }

            m_Items.Add(item);
        }
    }
}