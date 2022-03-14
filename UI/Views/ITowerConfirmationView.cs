using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface ITowerConfirmationView : IView, IFullscreenView
    {
        event Payload ContinueButtonClicked;
        event Payload ReturnToTownButtonClicked;

        void Construct(List<Item> items);
    }
}