using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class GambleViewController : ViewController<IGambleView>
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly ICurrencyRepository m_CurrencyRepository;
        private readonly InventoryComponent m_Inventory;
        private readonly List<Item> m_Buyback = new();

        private Currency m_Price;
        private List<Item> m_Assortment;

        public GambleViewController(IGambleView view, IItemRepository itemRepository, ICurrencyRepository currencyRepository) : base(view)
        {
            m_ItemRepository = itemRepository;
            m_CurrencyRepository = currencyRepository;
            m_Inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
        }

        protected override void OnInitialize()
        {
            View.Buy += OnBuy;
            View.Sell += OnSell;
            View.Gamble += OnGamble;
            View.Construct(Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel());
            RefreshAssortment();

            var experience = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience;
            experience.LevelUp += OnLevelUp;
            OnLevelUp(experience);
        }

        protected override void OnTerminate()
        {
            View.Buy -= OnBuy;
            View.Sell -= OnSell;
            View.Gamble -= OnGamble;

            Game.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.LevelUp -= OnLevelUp;
        }

        private void OnSell(Item item)
        {
            m_Inventory.Sell(item);

            if (!item.IsJunk)
            {
                m_Buyback.Add(item);
            }

            DisplayAssortment();
        }

        private void OnLevelUp(Experience experience)
        {
            m_Price = CalculatePrice(experience);
            View.UpdatePrice(m_Price);
        }

        private Currency CalculatePrice(Experience experience)
        {
            return m_CurrencyRepository.FindByType(CurrencyType.Gold)
                .Add(Mathf.Clamp(experience.Level / 10 * 1000, 250, 3000));
        }

        private void RefreshAssortment()
        {
            m_Assortment = m_ItemRepository
                .Random(Rng.Range(8, 12), Filter)
                .ToList();

            foreach (var item in m_Assortment)
            {
                item.ChangeOwner(Game.Instance.Character.Entity);
                item.PriceMultiplier = Mathf.Pow(8, (int) item.Rarity.Type * 0.3f);
            }

            DisplayAssortment();
        }

        private void DisplayAssortment()
        {
            View.Display(m_Assortment.Concat(m_Buyback).ToList());
        }

        private bool Filter(ItemData item)
        {
            if (!item.IsEnabled ||
                !item.Flags.HasFlag(ItemFlags.Droppable) ||
                item.Flags.HasFlag(ItemFlags.QuestReward) ||
                item.TypeId == Constants.c_ItemTypeIdSkillBook ||
                item.TypeId == Constants.c_ItemTypeIdRune ||
                item.TypeId == Constants.c_ItemTypeIdMinorRune ||
                item.TypeId == Constants.c_ItemTypeIdMajorRune)
            {
                return false;
            }

            if (item.TypeId == Constants.c_ItemTypeIdIngredient ||
                item.TypeId == Constants.c_ItemTypeIdConsumable ||
                item.TypeId == Constants.c_ItemTypeIdRelic)
            {
                return false;
            }

            if (item.RarityId == Constants.c_ItemRarityIdJunk ||
                item.RarityId == Constants.c_ItemRarityIdCommon ||
                item.RarityId == Constants.c_ItemRarityIdLegendary ||
                item.RarityId == Constants.c_ItemRarityIdMythic ||
                item.RarityId == Constants.c_ItemRarityIdVision ||
                item.RarityId == Constants.c_ItemRarityIdBlizzard)
            {
                return false;
            }

            var level = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.Level;

            if (level < 20 && item.RarityId == Constants.c_ItemRarityIdUnique)
            {
                return false;
            }

            if (level < 30 && (item.SetId > 0 || item.TypeId == Constants.c_ItemTypeIdEnchantment))
            {
                return false;
            }

            if (item.RarityId == Constants.c_ItemRarityIdUnique && !Rng.Test(0.25f))
            {
                return false;
            }

            return true;
        }

        private void OnBuy(Item item)
        {
            try
            {
                Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Buy(item);
                m_Assortment.Remove(item);
                m_Buyback.Remove(item);

                item.PriceMultiplier = 1;

                DisplayAssortment();

                AudioManager.Instance.PlayItemBuy();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }

        private void OnGamble()
        {
            try
            {
                Game.Instance.Character.Entity.GetComponent<CurrenciesComponent>().Withdraw(m_Price);

                RefreshAssortment();

                AudioManager.Instance.PlayItemSell();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}