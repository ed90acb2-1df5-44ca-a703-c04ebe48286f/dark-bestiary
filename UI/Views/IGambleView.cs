using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IGambleView : IView, IHideOnEscape
    {
        event Payload Gamble;
        event Payload<Item> Buy;

        void Construct(Character character, Currency price);

        void Display(List<Item> items);
    }
}