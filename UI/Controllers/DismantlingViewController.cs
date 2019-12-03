using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class DismantlingViewController : ViewController<IDismantlingView>
    {
        private readonly IItemRepository itemRepository;
        private readonly List<Item> toDismantle = new List<Item>();
        private readonly Character character;
        private readonly InventoryComponent inventory;
        private readonly EquipmentComponent equipment;

        public DismantlingViewController(IDismantlingView view, IItemRepository itemRepository,
            CharacterManager characterManager) : base(view)
        {
            this.itemRepository = itemRepository;
            this.character = characterManager.Character;
            this.inventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
            this.equipment = characterManager.Character.Entity.GetComponent<EquipmentComponent>();
        }

        protected override void OnInitialize()
        {
            View.ItemPlacing += OnItemPlacing;
            View.ItemRemoving += OnItemRemoving;
            View.DismantleButtonClicked += OnDismantleButtonClicked;
            View.ClearButtonClicked += OnClearButtonClicked;
            View.OkayButtonClicked += OnOkayButtonClicked;
            View.Construct(this.character);
        }

        protected override void OnTerminate()
        {
            View.ItemPlacing -= OnItemPlacing;
            View.ItemRemoving -= OnItemRemoving;
            View.DismantleButtonClicked -= OnDismantleButtonClicked;
            View.ClearButtonClicked -= OnClearButtonClicked;
            View.OkayButtonClicked -= OnOkayButtonClicked;
        }

        private void OnItemPlacing(Item item)
        {
            if (!item.IsDismantable || this.toDismantle.Contains(item))
            {
                return;
            }

            if (this.equipment.IsEquipped(item))
            {
                this.equipment.Unequip(item);
            }

            this.toDismantle.Add(item);
            View.DisplayDismantlingItems(this.toDismantle);
        }

        private void OnItemRemoving(Item item)
        {
            this.toDismantle.Remove(item);
            View.DisplayDismantlingItems(this.toDismantle);
        }

        private void OnDismantleButtonClicked()
        {
            if (this.toDismantle.Count == 0)
            {
                return;
            }

            var dismantled = this.toDismantle.SelectMany(CraftUtils.GetDismantleIngredients).ToList();

            this.inventory.Remove(this.toDismantle);
            this.inventory.Pickup(dismantled.Select(item => item.Clone()));

            this.toDismantle.Clear();

            View.DisplayDismantlingResult(dismantled);
        }

        private void OnOkayButtonClicked()
        {
            View.DisplayDismantlingItems(this.toDismantle);
        }

        private void OnClearButtonClicked()
        {
            this.toDismantle.Clear();
            View.DisplayDismantlingItems(this.toDismantle);
        }
    }
}