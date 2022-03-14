using System.Collections.Generic;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IMailboxView : IView, IHideOnEscape
    {
        event Payload NextPage;
        event Payload PreviousPage;
        event Payload TakeAll;
        event Payload<Item> Pick;
        event Payload<Item> Remove;

        void Refresh(List<Item> items, int currentPage, int totalPages);
    }
}