using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.UI.Views
{
    public interface ITowerVendorView : IView, IFullscreenView
    {
        event Action<Item> ItemSelected;
        event Action<Item> ItemDeselected;
        event Action ContinueButtonClicked;

        void Construct(List<Item> items);
    }
}