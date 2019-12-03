using System.Collections.Generic;
using DarkBestiary.Components;
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
        private readonly InventoryComponent inventory;

        private Item item;
        private List<RecipeIngredient> ingredients;

        protected ItemUpgradeViewController(string title, IItemUpgradeView view, CharacterManager characterManager) : base(view)
        {
            this.title = title;
            this.character = characterManager.Character;
            this.inventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
        }

        protected abstract List<RecipeIngredient> GetIngredients();
        protected abstract void Check(Item item);
        protected abstract void Upgrade(Item item);

        protected override void OnInitialize()
        {
            this.inventory.ItemPicked += OnItemPicked;
            this.inventory.ItemRemoved += OnItemRemoved;
            this.inventory.ItemStackCountChanged += OnItemStackCountChanged;

            View.ChanceTitle(this.title);
            View.ItemPlaced += OnUpgradeItemPlaced;
            View.ItemRemoved += OnUpgradeItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;

            this.ingredients = GetIngredients();
            this.item = this.inventory.CreateEmptyItem();

            View.Construct(this.character);
            Refresh();
        }

        protected override void OnTerminate()
        {
            this.inventory.ItemPicked -= OnItemPicked;
            this.inventory.ItemRemoved -= OnItemRemoved;
            this.inventory.ItemStackCountChanged -= OnItemStackCountChanged;

            View.ItemPlaced += OnUpgradeItemPlaced;
            View.ItemRemoved += OnUpgradeItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;
        }

        private void Refresh()
        {
            View.Refresh(this.item, this.ingredients);
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
            this.item = this.inventory.CreateEmptyItem();
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
                this.inventory.WithdrawIngredients(this.ingredients);
                Upgrade(this.item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }
    }
}