using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class RunesViewController : ViewController<IRunesView>
    {
        private readonly InventoryComponent inventoryComponent;

        private Item item;

        public RunesViewController(IRunesView view, CharacterManager characterManager) : base(view)
        {
            this.inventoryComponent = characterManager.Character.Entity.GetComponent<InventoryComponent>();
        }

        protected override void OnInitialize()
        {
            View.ItemDroppedIn += OnItemDroppedIn;
            View.ItemDroppedOut += OnItemDroppedOut;
            View.RuneDroppedIn += OnRuneDroppedIn;
            View.RuneDroppedOut += OnRuneDroppedOut;
            View.RuneRemoved += OnRuneRemoved;

            View.Construct(ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel());
            View.ChangeItem(Item.CreateEmpty());
        }

        protected override void OnTerminate()
        {
            View.ItemDroppedIn -= OnItemDroppedIn;
            View.ItemDroppedOut -= OnItemDroppedOut;
            View.RuneDroppedIn -= OnRuneDroppedIn;
            View.RuneDroppedOut -= OnRuneDroppedOut;
            View.RuneRemoved -= OnRuneRemoved;
        }

        private void OnRuneDroppedIn(Item rune, int targetIndex)
        {
            if (this.item == null || !rune.IsAnyRune || !CanBePlacedAtIndex(rune, targetIndex))
            {
                return;
            }

            var previousIndex = this.item.Runes.FindIndex(x => x == rune);

            if (previousIndex == -1)
            {
                this.inventoryComponent.Remove(rune, 1);

                if (!this.item.Runes[targetIndex].IsEmpty)
                {
                    this.inventoryComponent.Pickup(this.item.Runes[targetIndex]);
                }

                this.item.Runes[targetIndex] = rune.Clone().SetStack(1);
            }
            else
            {
                if (!CanBePlacedAtIndex(this.item.Runes[targetIndex], previousIndex) ||
                    !CanBePlacedAtIndex(this.item.Runes[previousIndex], targetIndex))
                {
                    return;
                }

                this.item.Runes[previousIndex] = this.item.Runes[targetIndex];
                this.item.Runes[targetIndex] = rune;
            }

            View.ChangeItem(this.item);
        }

        private bool CanBePlacedAtIndex(Item rune, int index)
        {
            if (rune.IsEmpty)
            {
                return true;
            }

            return GetRuneTypeIndex(Item.DetermineRuneTypeByIndex(index, this.item)) >= GetRuneTypeIndex(rune.Type.Type);
        }

        private static int GetRuneTypeIndex(ItemTypeType type)
        {
            switch (type)
            {
                case ItemTypeType.MinorRune:
                    return 0;
                case ItemTypeType.Rune:
                    return 1;
                case ItemTypeType.MajorRune:
                    return 2;
            }

            return 0;
        }

        private void OnRuneRemoved(int index)
        {
            this.inventoryComponent.Pickup(this.item.Runes[index]);
            this.item.Runes[index] = Item.CreateEmpty();
            View.ChangeItem(this.item);
        }

        private void OnRuneDroppedOut(int index)
        {
            this.item.Runes[index] = Item.CreateEmpty();
            View.ChangeItem(this.item);
        }

        private void OnItemDroppedIn(Item item)
        {
            if (item.Runes.Count == 0)
            {
                return;
            }

            this.item = item;
            View.ChangeItem(this.item);
        }

        private void OnItemDroppedOut(Item item)
        {
            this.item = Item.CreateEmpty();
            View.ChangeItem(this.item);
        }
    }
}