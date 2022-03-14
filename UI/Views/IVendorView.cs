using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IVendorView : IView, IHideOnEscape
    {
        event Payload SellJunk;
        event Payload<Item> SellingItem;
        event Payload<Item> BuyingItem;

        void Construct(InventoryPanel inventoryPanel, List<VendorPanel.Category> categories);
        void RefreshAssortment(List<Item> assortment);
        void MarkExpensive(Item item);
        void MarkAffordable(Item item);
    }
}