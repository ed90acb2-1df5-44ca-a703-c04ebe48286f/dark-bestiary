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
            View.Construct(this.character, this.stash.Inventory);
        }

        protected override void OnTerminate()
        {
            View.Deposit -= OnItemDeposit;
            View.Withdraw -= OnItemWithdraw;
        }

        private void OnItemWithdraw(Item item)
        {
            this.stash.Inventory.Remove(item);
            this.inventory.Pickup(item);
        }

        private void OnItemDeposit(Item item)
        {
            if (this.equipment.IsEquipped(item))
            {
                this.equipment.Unequip(item);
            }

            this.stash.Inventory.Pickup(item);
            this.inventory.Remove(item);
        }
    }
}