using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
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
        private readonly Character character;
        private readonly Currency price;

        private List<Item> assortment;

        public GambleViewController(IGambleView view, IItemRepository itemRepository,
            ICurrencyRepository currencyRepository, CharacterManager characterManager) : base(view)
        {
            this.itemRepository = itemRepository;
            this.character = characterManager.Character;

            var power = this.character.Entity.GetComponent<ExperienceComponent>().Experience.Level / 10;

            this.price = currencyRepository.FindByType(CurrencyType.Gold)
                .Add(200 + Math.Pow(5, Mathf.Clamp(2 + power, 1, 5)));
        }

        protected override void OnInitialize()
        {
            View.Buy += OnBuy;
            View.Gamble += OnGamble;
            View.Construct(this.character, this.price);
        }

        protected override void OnTerminate()
        {
            View.Buy -= OnBuy;
            View.Gamble -= OnGamble;
        }

        private void RefreshAssortment()
        {
            this.assortment = this.itemRepository.Random(RNG.Range(5, 10), item =>
                    !item.Flags.HasFlag(ItemFlags.QuestReward) &&
                    item.IsEnabled &&
                    item.TypeId != Constants.ItemTypeIdIngredient &&
                    item.TypeId != Constants.ItemTypeIdConsumable &&
                    item.RarityId != Constants.ItemRarityIdJunk &&
                    item.RarityId != Constants.ItemRarityIdCommon &&
                    item.RarityId != Constants.ItemRarityIdLegendary &&
                    item.RarityId != Constants.ItemRarityIdBlizzard &&
                    (item.Level > 3 ||
                     Item.MatchDropByMonsterLevel(
                         item.Level, this.character.Entity.GetComponent<ExperienceComponent>().Experience.Level)))
                .ToList();

            foreach (var item in this.assortment)
            {
                item.ChangeOwner(this.character.Entity);
                item.PriceMultiplier = Mathf.Pow(8, (int) item.Rarity.Type * 0.3f);
            }

            View.Display(this.assortment);
        }

        private void OnBuy(Item item)
        {
            try
            {
                this.character.Entity.GetComponent<InventoryComponent>().Buy(item);
                this.assortment.Remove(item);

                item.PriceMultiplier = 1;

                View.Display(this.assortment);

                AudioManager.Instance.PlayItemBuy();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
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
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }
    }
}