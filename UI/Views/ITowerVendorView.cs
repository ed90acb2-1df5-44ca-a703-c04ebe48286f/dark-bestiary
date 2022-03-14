using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ITowerVendorView : IView, IFullscreenView
    {
        event Payload<Item> ItemSelected;
        event Payload<Item> ItemDeselected;
        event Payload ContinueButtonClicked;

        void Construct(List<Item> items);
    }
}