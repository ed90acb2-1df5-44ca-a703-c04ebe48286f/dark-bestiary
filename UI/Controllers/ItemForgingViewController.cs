using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class ItemForgingViewController : ViewController<IItemForgingView>
    {
        private readonly Character character;
        private readonly InventoryComponent inventory;
        private readonly EquipmentComponent equipment;

        private List<RecipeIngredient> ingredients;
        private Item item;

        public ItemForgingViewController(IItemForgingView view, CharacterManager characterManager) : base(view)
        {
            this.character = characterManager.Character;
            this.inventory = this.character.Entity.GetComponent<InventoryComponent>();
            this.equipment = this.character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            this.equipment.ItemEquipped += OnItemEquipped;

            View.ItemPlaced += OnItemPlaced;
            View.ItemRemoved += OnItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;
            View.Construct(this.character);
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
            if (!item.IsWeapon && !item.IsArmor && !item.IsJewelry || item.IsGem)
            {
                return;
            }

            if (item.ForgeLevel >= Item.MaxForgeLevel)
            {
                UiErrorFrame.Instance.Push(I18N.Instance.Get("exception_max_forging_level"));
                return;
            }

            this.item = item;

            Refresh(item);
        }

        private void Refresh(Item item)
        {
            var upgraded = item.Clone();
            upgraded.ForgeLevel += 1;

            this.ingredients = CraftUtils.GetForgeIngredients(upgraded)
                .GroupBy(i => i.Id)
                .Select(group => new RecipeIngredient(group.First(), group.Sum(i => i.StackCount)))
                .ToList();

            View.Refresh(item, upgraded, this.ingredients);
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
                UiErrorFrame.Instance.Push(I18N.Instance.Get("exception_max_forging_level"));
                return;
            }

            try
            {
                this.inventory.WithdrawIngredients(this.ingredients);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
                return;
            }

            this.item.Forge();

            Refresh(this.item);
        }
    }
}