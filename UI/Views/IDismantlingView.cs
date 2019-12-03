﻿using System.Collections.Generic;
using DarkBestiary.Items;
 using DarkBestiary.Messaging;

 namespace DarkBestiary.UI.Views
{
    public interface IDismantlingView : IView, IHideOnEscape
    {
        event Payload OkayButtonClicked;
        event Payload DismantleButtonClicked;
        event Payload ClearButtonClicked;
        event Payload<Item> ItemPlacing;
        event Payload<Item> ItemRemoving;

        void Construct(Character character);

        void DisplayDismantlingItems(IEnumerable<Item> items);

        void DisplayDismantlingResult(IEnumerable<Item> items);
    }
}