using System;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IRuneInscriptionView : IView
    {
        event Action<Item> ItemDroppedIn;
        event Action<Item> ItemDroppedOut;
        event Action<Item, int> InscriptionDroppedIn;
        event Action<int> InscriptionDroppedOut;
        event Action<int> InscriptionRemoved;

        void Construct(InventoryPanel inventoryPanel);
        void ChangeItem(Item item);
    }
}