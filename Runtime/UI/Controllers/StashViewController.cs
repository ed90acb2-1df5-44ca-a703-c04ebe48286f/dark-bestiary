using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class StashViewController : ViewController<IStashView>
    {
        private readonly Stash m_Stash;
        private readonly InventoryComponent m_Inventory;
        private readonly EquipmentComponent m_Equipment;
        private readonly Character m_Character;

        public StashViewController(IStashView view, Stash stash) : base(view)
        {
            m_Stash = stash;
            m_Character = Game.Instance.Character;
            m_Inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
            m_Equipment = Game.Instance.Character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            View.Deposit += OnItemDeposit;
            View.Withdraw += OnItemWithdraw;
            View.DepositIngredients += OnDepositMaterials;
            View.WithdrawIngredients += OnWithdrawIngredients;
            View.Construct(Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel(), m_Character, m_Stash.Inventories);
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
            foreach (var item in m_Inventory.Items.Where(item => item.Type?.Type == ItemTypeType.Ingredient).ToList())
            {
                m_Inventory.Remove(item);
                m_Stash.Inventories[inventoryIndex].Pickup(item);
            }
        }

        private void OnWithdrawIngredients(int inventoryIndex)
        {
            var ingredients = m_Stash.Inventories[inventoryIndex].Items
                .Where(item => item.Type?.Type == ItemTypeType.Ingredient)
                .Take(m_Inventory.GetFreeSlotCount())
                .ToList();

            foreach (var item in ingredients)
            {
                m_Stash.Inventories[inventoryIndex].Remove(item);
                m_Inventory.Pickup(item);
            }
        }

        private void OnItemWithdraw(Item item, int inventoryIndex)
        {
            m_Stash.Inventories[inventoryIndex].Remove(item);
            m_Inventory.Pickup(item);
        }

        private void OnItemDeposit(Item item, int inventoryIndex)
        {
            if (m_Equipment.IsEquipped(item))
            {
                m_Equipment.Unequip(item);
            }

            m_Stash.Inventories[inventoryIndex].Pickup(item);
            m_Inventory.Remove(item);
        }
    }
}