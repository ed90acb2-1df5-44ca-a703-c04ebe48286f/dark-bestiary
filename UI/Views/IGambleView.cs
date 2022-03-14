using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IGambleView : IView, IHideOnEscape
    {
        event Payload Gamble;
        event Payload<Item> Buy;
        event Payload<Item> Sell;

        void Construct(InventoryPanel inventoryPanel);
        void UpdatePrice(Currency price);
        void Display(List<Item> items);
    }
}