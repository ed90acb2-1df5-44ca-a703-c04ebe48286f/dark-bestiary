using System;
using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IDismantlingView : IView, IHideOnEscape
    {
        event Action OkayButtonClicked;
        event Action DismantleButtonClicked;
        event Action ClearButtonClicked;
        event Action<RarityType> PlaceItems;
        event Action<Item> ItemPlacing;
        event Action<Item> ItemRemoving;

        void Construct(InventoryPanel inventoryPanel);
        void DisplayDismantlingItems(IEnumerable<Item> items);
        void DisplayDismantlingResult(IEnumerable<Item> items);
    }
}