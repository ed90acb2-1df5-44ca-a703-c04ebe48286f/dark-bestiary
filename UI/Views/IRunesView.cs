using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IRunesView : IView
    {
        event Payload<Item> ItemDroppedIn;
        event Payload<Item> ItemDroppedOut;
        event Payload<Item, int> RuneDroppedIn;
        event Payload<int> RuneDroppedOut;
        event Payload<int> RuneRemoved;

        void Construct(InventoryPanel inventoryPanel);
        void ChangeItem(Item item);
    }
}