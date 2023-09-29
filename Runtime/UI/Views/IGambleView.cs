using System;
using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IGambleView : IView, IHideOnEscape
    {
        event Action Gamble;
        event Action<Item> Buy;
        event Action<Item> Sell;

        void Construct(InventoryPanel inventoryPanel);
        void UpdatePrice(Currency price);
        void Display(List<Item> items);
    }
}