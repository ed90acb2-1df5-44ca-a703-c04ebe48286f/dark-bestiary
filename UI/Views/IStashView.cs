using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;

namespace DarkBestiary.UI.Views
{
    public interface IStashView : IView, IHideOnEscape
    {
        event Payload<Item> Deposit;
        event Payload<Item> Withdraw;

        void Construct(Character character, InventoryComponent stashInventory);
    }
}