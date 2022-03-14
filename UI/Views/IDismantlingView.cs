using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IDismantlingView : IView, IHideOnEscape
    {
        event Payload OkayButtonClicked;
        event Payload DismantleButtonClicked;
        event Payload ClearButtonClicked;
        event Payload<RarityType> PlaceItems;
        event Payload<Item> ItemPlacing;
        event Payload<Item> ItemRemoving;

        void Construct(InventoryPanel inventoryPanel);
        void DisplayDismantlingItems(IEnumerable<Item> items);
        void DisplayDismantlingResult(IEnumerable<Item> items);
    }
}