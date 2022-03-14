using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class StashViewController : ViewController<IStashView>
    {
        private readonly Stash stash;
        private readonly InventoryComponent inventory;
        private readonly EquipmentComponent equipment;
        private readonly Character character;

        public StashViewController(IStashView view, Stash stash, CharacterManager characterManager) : base(view)
        {
            this.stash = stash;
            this.character = characterManager.Character;
            this.inventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
            this.equipment = characterManager.Character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            View.Deposit += OnItemDeposit;
            View.Withdraw += OnItemWithdraw;
            View.DepositIngredients += OnDepositMaterials;
            View.WithdrawIngredients += OnWithdrawIngredients;
            View.Construct(ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel(), this.character, this.stash.Inventories);
        }

        protected override void OnTerminate()
        {
            View.Deposit -= OnItemDeposit;
            View.Withdraw -= OnItemWithdraw;
            View.DepositIngredients -= OnDepositMaterials;
            View.WithdrawIngredients -= OnWithdrawIngredients;
        }

        private void OnDepositMaterials(int inventoryIndex)
        {
            foreach (var item in this.inventory.Items.Where(item => item.Type?.Type == ItemTypeType.Ingredient).ToList())
            {
                this.inventory.Remove(item);
                this.stash.Inventories[inventoryIndex].Pickup(item);
            }
        }

        private void OnWithdrawIngredients(int inventoryIndex)
        {
            var ingredients = this.stash.Inventories[inventoryIndex].Items
                .Where(item => item.Type?.Type == ItemTypeType.Ingredient)
                .Take(this.inventory.GetFreeSlotCount())
                .ToList();

            foreach (var item in ingredients)
            {
                this.stash.Inventories[inventoryIndex].Remove(item);
                this.inventory.Pickup(item);
            }
        }

        private void OnItemWithdraw(Item item, int inventoryIndex)
        {
            this.stash.Inventories[inventoryIndex].Remove(item);
            this.inventory.Pickup(item);
        }

        private void OnItemDeposit(Item item, int inventoryIndex)
        {
            if (this.equipment.IsEquipped(item))
            {
                this.equipment.Unequip(item);
            }

            this.stash.Inventories[inventoryIndex].Pickup(item);
            this.inventory.Remove(item);
        }
    }
}