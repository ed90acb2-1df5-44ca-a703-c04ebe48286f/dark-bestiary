using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IVendorView : IView, IHideOnEscape
    {
        event Payload SellJunk;
        event Payload<Item> SellingItem;
        event Payload<Item> BuyingItem;

        void RefreshAssortment(List<Item> assortment);

        void MarkExpensive(Item item);

        void MarkAffordable(Item item);

        void Construct(Character character);
    }
}