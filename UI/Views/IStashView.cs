using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IStashView : IView, IHideOnEscape
    {
        event Payload<Item, int> Deposit;
        event Payload<Item, int> Withdraw;
        event Payload<int> DepositIngredients;
        event Payload<int> WithdrawIngredients;

        void Construct(InventoryPanel inventoryPanel, Character character, InventoryComponent[] inventories);
    }
}