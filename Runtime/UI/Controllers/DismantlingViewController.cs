using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class DismantlingViewController : ViewController<IDismantlingView>
    {
        private readonly List<Item> m_ToDismantle = new();
        private readonly InventoryComponent m_Inventory;
        private readonly EquipmentComponent m_Equipment;

        public DismantlingViewController(IDismantlingView view) : base(view)
        {
            m_Inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
            m_Equipment = Game.Instance.Character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            View.ItemPlacing += OnItemPlacing;
            View.ItemRemoving += OnItemRemoving;
            View.DismantleButtonClicked += OnDismantleButtonClicked;
            View.ClearButtonClicked += OnClearButtonClicked;
            View.OkayButtonClicked += OnOkayButtonClicked;
            View.PlaceItems += OnPlaceItems;

            View.Construct(Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel());
        }

        protected override void OnTerminate()
        {
            View.ItemPlacing -= OnItemPlacing;
            View.ItemRemoving -= OnItemRemoving;
            View.DismantleButtonClicked -= OnDismantleButtonClicked;
            View.ClearButtonClicked -= OnClearButtonClicked;
            View.OkayButtonClicked -= OnOkayButtonClicked;
            View.PlaceItems -= OnPlaceItems;
        }

        private void OnPlaceItems(RarityType rarity)
        {
            m_ToDismantle.Clear();

            foreach (var item in m_Inventory.Items.Where(i => i.Rarity?.Type == rarity))
            {
                if (!item.IsDismantable)
                {
                    continue;
                }

                m_ToDismantle.Add(item);
            }

            View.DisplayDismantlingItems(m_ToDismantle);
        }

        private void OnItemPlacing(Item item)
        {
            if (!item.IsDismantable || m_ToDismantle.Contains(item))
            {
                return;
            }

            if (m_Equipment.IsEquipped(item))
            {
                m_Equipment.Unequip(item);
            }

            m_ToDismantle.Add(item);
            View.DisplayDismantlingItems(m_ToDismantle);
        }

        private void OnItemRemoving(Item item)
        {
            m_ToDismantle.Remove(item);
            View.DisplayDismantlingItems(m_ToDismantle);
        }

        private void OnDismantleButtonClicked()
        {
            if (m_ToDismantle.Count == 0)
            {
                return;
            }

            var dismantled = m_ToDismantle.SelectMany(CraftUtils.GetDismantleIngredients).ToList();

            m_Inventory.Remove(m_ToDismantle);
            m_Inventory.Pickup(dismantled.Select(item => item.Clone()));

            m_ToDismantle.Clear();

            View.DisplayDismantlingResult(dismantled);
        }

        private void OnOkayButtonClicked()
        {
            View.DisplayDismantlingItems(m_ToDismantle);
        }

        private void OnClearButtonClicked()
        {
            m_ToDismantle.Clear();
            View.DisplayDismantlingItems(m_ToDismantle);
        }
    }
}