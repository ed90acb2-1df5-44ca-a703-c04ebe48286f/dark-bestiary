using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class RuneInscriptionViewController : ViewController<IRuneInscriptionView>
    {
        private readonly InventoryComponent m_InventoryComponent;

        private Item m_Item;

        public RuneInscriptionViewController(IRuneInscriptionView view) : base(view)
        {
            m_InventoryComponent = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
        }

        protected override void OnInitialize()
        {
            View.ItemDroppedIn += OnItemDroppedIn;
            View.ItemDroppedOut += OnItemDroppedOut;
            View.InscriptionDroppedIn += OnInscriptionDroppedIn;
            View.InscriptionDroppedOut += OnInscriptionDroppedOut;
            View.InscriptionRemoved += OnInscriptionRemoved;

            View.Construct(Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel());
            View.ChangeItem(Item.CreateEmpty());
        }

        protected override void OnTerminate()
        {
            View.ItemDroppedIn -= OnItemDroppedIn;
            View.ItemDroppedOut -= OnItemDroppedOut;
            View.InscriptionDroppedIn -= OnInscriptionDroppedIn;
            View.InscriptionDroppedOut -= OnInscriptionDroppedOut;
            View.InscriptionRemoved -= OnInscriptionRemoved;
        }

        private void OnInscriptionDroppedIn(Item rune, int targetIndex)
        {
            if (m_Item == null || !rune.IsAnyRune || !CanBePlacedAtIndex(rune, targetIndex))
            {
                return;
            }

            var previousIndex = m_Item.Runes.FindIndex(x => x == rune);

            if (previousIndex == -1)
            {
                m_InventoryComponent.Remove(rune, 1);

                if (!m_Item.Runes[targetIndex].IsEmpty)
                {
                    m_InventoryComponent.Pickup(m_Item.Runes[targetIndex]);
                }

                m_Item.Runes[targetIndex] = rune.Clone().SetStack(1);
            }
            else
            {
                if (!CanBePlacedAtIndex(m_Item.Runes[targetIndex], previousIndex) ||
                    !CanBePlacedAtIndex(m_Item.Runes[previousIndex], targetIndex))
                {
                    return;
                }

                m_Item.Runes[previousIndex] = m_Item.Runes[targetIndex];
                m_Item.Runes[targetIndex] = rune;
            }

            View.ChangeItem(m_Item);
        }

        private bool CanBePlacedAtIndex(Item rune, int index)
        {
            if (rune.IsEmpty)
            {
                return true;
            }

            return GetRuneTypeIndex(Item.DetermineRuneTypeByIndex(index, m_Item)) >= GetRuneTypeIndex(rune.Type.Type);
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

        private void OnInscriptionRemoved(int index)
        {
            m_InventoryComponent.Pickup(m_Item.Runes[index]);
            m_Item.Runes[index] = Item.CreateEmpty();
            View.ChangeItem(m_Item);
        }

        private void OnInscriptionDroppedOut(int index)
        {
            m_Item.Runes[index] = Item.CreateEmpty();
            View.ChangeItem(m_Item);
        }

        private void OnItemDroppedIn(Item item)
        {
            if (item.Runes.Count == 0)
            {
                return;
            }

            m_Item = item;
            View.ChangeItem(m_Item);
        }

        private void OnItemDroppedOut(Item item)
        {
            m_Item = Item.CreateEmpty();
            View.ChangeItem(m_Item);
        }
    }
}