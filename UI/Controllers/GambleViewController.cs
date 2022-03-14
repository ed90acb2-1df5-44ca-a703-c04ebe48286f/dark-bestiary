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
        private readonly IItemRepository itemRepository;
        private readonly ICurrencyRepository currencyRepository;
        private readonly Character character;
        private readonly InventoryComponent inventory;
        private readonly List<Item> buyback = new List<Item>();

        private Currency price;
        private List<Item> assortment;

        public GambleViewController(IGambleView view, IItemRepository itemRepository,
            ICurrencyRepository currencyRepository, CharacterManager characterManager) : base(view)
        {
            this.itemRepository = itemRepository;
            this.currencyRepository = currencyRepository;
            this.character = characterManager.Character;
            this.inventory = this.character.Entity.GetComponent<InventoryComponent>();
        }

        protected override void OnInitialize()
        {
            View.Buy += OnBuy;
            View.Sell += OnSell;
            View.Gamble += OnGamble;
            View.Construct(ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel());
            RefreshAssortment();

            var experience = this.character.Entity.GetComponent<ExperienceComponent>().Experience;
            experience.LevelUp += OnLevelUp;
            OnLevelUp(experience);
        }

        protected override void OnTerminate()
        {
            View.Buy -= OnBuy;
            View.Sell -= OnSell;
            View.Gamble -= OnGamble;

            this.character.Entity.GetComponent<ExperienceComponent>().Experience.LevelUp -= OnLevelUp;
        }

        private void OnSell(Item item)
        {
            this.inventory.Sell(item);

            if (!item.IsJunk)
            {
                this.buyback.Add(item);
            }

            DisplayAssortment();
        }

        private void OnLevelUp(Experience experience)
        {
            this.price = CalculatePrice(experience);
            View.UpdatePrice(this.price);
        }

        private Currency CalculatePrice(Experience experience)
        {
            return this.currencyRepository.FindByType(CurrencyType.Gold)
                .Add(Mathf.Clamp(experience.Level / 10 * 1000, 250, 3000));
        }

        private void RefreshAssortment()
        {
            this.assortment = this.itemRepository
                .Random(RNG.Range(8, 12), Filter)
                .ToList();

            foreach (var item in this.assortment)
            {
                item.ChangeOwner(this.character.Entity);
                item.PriceMultiplier = Mathf.Pow(8, (int) item.Rarity.Type * 0.3f);
            }

            DisplayAssortment();
        }

        private void DisplayAssortment()
        {
            View.Display(this.assortment.Concat(this.buyback).ToList());
        }

        private bool Filter(ItemData item)
        {
            if (!item.IsEnabled ||
                !item.Flags.HasFlag(ItemFlags.Droppable) ||
                item.Flags.HasFlag(ItemFlags.QuestReward) ||
                item.TypeId == Constants.ItemTypeIdSkillBook)
            {
                return false;
            }

            if (this.character.Data.UnlockedRecipes.Contains(item.BlueprintRecipeId))
            {
                return false;
            }

            if (item.TypeId == Constants.ItemTypeIdIngredient ||
                item.TypeId == Constants.ItemTypeIdConsumable ||
                item.TypeId == Constants.ItemTypeIdRelic)
            {
                return false;
            }

            if (item.RarityId == Constants.ItemRarityIdJunk ||
                item.RarityId == Constants.ItemRarityIdCommon ||
                item.RarityId == Constants.ItemRarityIdLegendary ||
                item.RarityId == Constants.ItemRarityIdMythic ||
                item.RarityId == Constants.ItemRarityIdVision ||
                item.RarityId == Constants.ItemRarityIdBlizzard)
            {
                return false;
            }

            var level = this.character.Entity.GetComponent<ExperienceComponent>().Experience.Level;

            if (level < 20 && item.RarityId == Constants.ItemRarityIdUnique)
            {
                return false;
            }

            if (level < 30 && (item.SetId > 0 || item.TypeId == Constants.ItemTypeIdEnchantment))
            {
                return false;
            }

            if (item.RarityId == Constants.ItemRarityIdUnique && !RNG.Test(0.25f))
            {
                return false;
            }

            return Item.MatchDropByMonsterLevel(item, level);
        }

        private void OnBuy(Item item)
        {
            try
            {
                this.character.Entity.GetComponent<InventoryComponent>().Buy(item);
                this.assortment.Remove(item);
                this.buyback.Remove(item);

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
                this.character.Entity.GetComponent<CurrenciesComponent>().Withdraw(this.price);

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