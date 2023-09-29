using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Events;
using DarkBestiary.Exceptions;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public abstract class ItemUpgradeViewController : ViewController<IItemUpgradeView>
    {
        private readonly string m_Title;
        private readonly Character m_Character;
        private readonly InventoryComponent m_IngredientInventory;
        private readonly CurrenciesComponent m_Currencies;

        private Item m_Item;
        private List<RecipeIngredient> m_Ingredients;
        private Currency? m_Cost;

        protected ItemUpgradeViewController(string title, IItemUpgradeView view) : base(view)
        {
            m_Title = title;
            m_Character = Game.Instance.Character;
            m_IngredientInventory = Stash.Instance.GetIngredientsInventory();
            m_Currencies = m_Character.Entity.GetComponent<CurrenciesComponent>();
        }

        protected abstract List<RecipeIngredient> GetIngredients();
        protected abstract Currency? GetCost();
        protected abstract void Check(Item item);
        protected abstract void Upgrade(Item item);

        protected override void OnInitialize()
        {
            m_IngredientInventory.ItemPicked += OnItemPicked;
            m_IngredientInventory.ItemRemoved += OnItemRemoved;
            m_IngredientInventory.ItemStackCountChanged += OnItemStackCountChanged;

            View.ChangeTitle(m_Title);
            View.ItemPlaced += OnUpgradeItemPlaced;
            View.ItemRemoved += OnUpgradeItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;

            m_Ingredients = GetIngredients();
            m_Cost = GetCost();
            m_Item = m_IngredientInventory.CreateEmptyItem();

            View.Construct(
                Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel(),
                m_Character.Entity.GetComponent<InventoryComponent>(),
                m_IngredientInventory
            );

            Refresh();
        }

        protected override void OnTerminate()
        {
            m_IngredientInventory.ItemPicked -= OnItemPicked;
            m_IngredientInventory.ItemRemoved -= OnItemRemoved;
            m_IngredientInventory.ItemStackCountChanged -= OnItemStackCountChanged;

            View.ItemPlaced += OnUpgradeItemPlaced;
            View.ItemRemoved += OnUpgradeItemRemoved;
            View.UpgradeButtonClicked += OnUpgradeButtonClicked;
        }

        private void Refresh()
        {
            View.Refresh(m_Item, m_Ingredients);
            View.RefreshCost(m_Cost);
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

            m_Item = item;
            Refresh();
        }

        private void OnUpgradeItemRemoved(Item item)
        {
            m_Item = m_IngredientInventory.CreateEmptyItem();
            Refresh();
        }

        private void OnUpgradeButtonClicked()
        {
            if (m_Item.IsEmpty)
            {
                return;
            }

            try
            {
                Check(m_Item);

                if (m_Cost != null)
                {
                    m_Currencies.Withdraw(m_Cost);
                    m_Cost = GetCost();
                }

                if (m_Ingredients != null)
                {
                    m_IngredientInventory.WithdrawIngredients(m_Ingredients);
                }

                Upgrade(m_Item);
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}