using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public abstract class ItemUpgradeViewController : ViewController<IItemUpgradeView>
    {
        private readonly string title;
        private readonly Character character;
        private readonly InventoryComponent ingredientInventory;
        private readonly CurrenciesComponent currencies;

        private Item item;
        private List<RecipeIngredient> ingredients;
        private Currency cost;

        protected ItemUpgradeViewController(string title, IItemUpgradeView view, CharacterManager characterManager) : base(view)
        {
            this.title = title;
            this.character = characterManager.Character;
            this.ingredientInventory = Stash.Instance.GetIngredientsInventory();
            this.currencies = characterManager.Character.Entity.GetComponent<CurrenciesComponent>();
        }

        protected abstract List<RecipeIngredient> GetIngredients();
        protected abstract Currency GetCost();
        protected abstract void Check(Item item);
        protected abstract void Upgrade(Item item);

        protected override void OnInitialize()
        {
            this.ingredientInventory.ItemPicked += OnItemPicked;
            this.ingredientInventory.ItemRemoved += OnItemRemoved;
            this.ingredientInventory.ItemStackCountChanged += OnItemStackCountChanged;

            View.ChangeTitle(this.title);
            View.ItemPlaced += OnUpgradeItemPlaced;
            View.ItemRemoved += OnUpgradeItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;

            this.ingredients = GetIngredients();
            this.cost = GetCost();
            this.item = this.ingredientInventory.CreateEmptyItem();

            View.Construct(
                ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel(),
                this.character.Entity.GetComponent<InventoryComponent>(),
                this.ingredientInventory
            );

            Refresh();
        }

        protected override void OnTerminate()
        {
            this.ingredientInventory.ItemPicked -= OnItemPicked;
            this.ingredientInventory.ItemRemoved -= OnItemRemoved;
            this.ingredientInventory.ItemStackCountChanged -= OnItemStackCountChanged;

            View.ItemPlaced += OnUpgradeItemPlaced;
            View.ItemRemoved += OnUpgradeItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;
        }

        private void Refresh()
        {
            View.Refresh(this.item, this.ingredients);
            View.RefreshCost(this.cost);
        }

        private void OnItemStackCountChanged(ItemStackCountChangedEventData data)
        {
            Refresh();
        }

        private void OnItemRemoved(ItemRemovedEventData data)
        {
            Refresh();
        }

        private void OnItemPicked(ItemPickupEventData data)
        {
            Refresh();
        }

        private void OnUpgradeItemPlaced(Item item)
        {
            if (!item.IsSocketable)
            {
                return;
            }

            this.item = item;
            Refresh();
        }

        private void OnUpgradeItemRemoved(Item item)
        {
            this.item = this.ingredientInventory.CreateEmptyItem();
            Refresh();
        }

        private void OnUpgradeButtonClicked()
        {
            if (this.item.IsEmpty)
            {
                return;
            }

            try
            {
                Check(this.item);

                if (this.cost != null)
                {
                    this.currencies.Withdraw(this.cost);
                    this.cost = GetCost();
                }

                if (this.ingredients != null)
                {
                    this.ingredientInventory.WithdrawIngredients(this.ingredients);
                }

                Upgrade(this.item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}