using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.UI.Views
{
    public interface IMailboxView : IView, IHideOnEscape
    {
        event Action NextPage;
        event Action PreviousPage;
        event Action TakeAll;
        event Action<Item> Pick;
        event Action<Item> Remove;

        void Refresh(List<Item> items, int currentPage, int totalPages);
    }
}