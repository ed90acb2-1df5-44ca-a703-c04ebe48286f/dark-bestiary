using System;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface IStashView : IView, IHideOnEscape
    {
        event Action<Item, int> Deposit;
        event Action<Item, int> Withdraw;
        event Action<int> DepositIngredients;
        event Action<int> WithdrawIngredients;

        void Construct(InventoryPanel inventoryPanel, Character character, InventoryComponent[] inventories);
    }
}