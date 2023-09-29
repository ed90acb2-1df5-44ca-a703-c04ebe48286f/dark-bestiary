using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.UI.Views
{
    public interface ITowerConfirmationView : IView, IFullscreenView
    {
        event Action ContinueButtonClicked;
        event Action ReturnToTownButtonClicked;

        void Construct(List<Item> items);
    }
}