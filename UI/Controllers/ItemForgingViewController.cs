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
    public class ItemForgingViewController : ViewController<IItemForgingView>
    {
        private readonly ICurrencyRepository currencyRepository;
        private readonly Character character;
        private readonly InventoryComponent ingredientsInventory;
        private readonly CurrenciesComponent currencies;
        private readonly EquipmentComponent equipment;

        private List<RecipeIngredient> ingredients;
        private Currency cost;
        private int uses;
        private Item item;

        public ItemForgingViewController(IItemForgingView view, ICurrencyRepository currencyRepository, CharacterManager characterManager) : base(view)
        {
            this.currencyRepository = currencyRepository;
            this.character = characterManager.Character;
            this.ingredientsInventory = Stash.Instance.GetIngredientsInventory();
            this.equipment = this.character.Entity.GetComponent<EquipmentComponent>();
            this.currencies = this.character.Entity.GetComponent<CurrenciesComponent>();
        }

        protected override void OnInitialize()
        {
            this.equipment.ItemEquipped += OnItemEquipped;

            View.ItemPlaced += OnItemPlaced;
            View.ItemRemoved += OnItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;

            View.Construct(
                ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel(),
                this.character.Entity.GetComponent<InventoryComponent>(),
                Stash.Instance.GetIngredientsInventory()
            );

            View.RefreshCost(null);
        }

        protected override void OnTerminate()
        {
            this.equipment.ItemEquipped -= OnItemEquipped;

            View.ItemPlaced -= OnItemPlaced;
            View.ItemRemoved -= OnItemRemoved;
        }

        private void OnItemEquipped(Item item)
        {
            OnItemRemoved(item);
        }

        private void OnItemPlaced(Item item)
        {
            if (!item.IsEquipment)
            {
                return;
            }

            this.item = item;

            Refresh(item);
        }

        public void Reset()
        {
            this.uses = 0;
            this.cost = null;

            View.Cleanup();
            View.RefreshCost(this.cost);
        }

        private void Refresh(Item item)
        {
            var upgraded = item.Clone();
            upgraded.ForgeLevel = Mathf.Min(upgraded.ForgeLevel + 1, Item.MaxForgeLevel);

            if (Game.Instance.IsVisions)
            {
                this.cost = this.uses > 0 ? this.currencyRepository.FindByType(CurrencyType.VisionFragment).Add(this.uses * 5) : null;
                this.ingredients = new List<RecipeIngredient>();
            }
            else
            {
                this.cost = null;
                this.ingredients = CraftUtils.GetForgeIngredients(upgraded)
                    .GroupBy(i => i.Id)
                    .Select(group => new RecipeIngredient(group.First(), group.Sum(i => i.StackCount)))
                    .ToList();
            }

            View.Refresh(item, upgraded, this.ingredients);
            View.RefreshCost(this.cost);
        }

        private void OnItemRemoved(Item item)
        {
            this.item = null;
            View.Cleanup();
        }

        private void OnUpgradeButtonClicked()
        {
            if (this.item == null)
            {
                return;
            }

            if (this.item.ForgeLevel >= Item.MaxForgeLevel)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_max_forging_level"));
                return;
            }

            if (this.item.ForgeLevel >= Item.MaxForgeLevel)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_max_forging_level"));
                return;
            }

            try
            {
                if (this.cost != null)
                {
                    this.currencies.Withdraw(this.cost);
                }

                this.ingredientsInventory.WithdrawIngredients(this.ingredients);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
                return;
            }

            this.item.Forge();

            this.uses++;

            Refresh(this.item);
        }
    }
}