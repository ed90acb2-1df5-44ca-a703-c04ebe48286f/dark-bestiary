using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;
using JetBrains.Annotations;

namespace DarkBestiary.UI.Views
{
    public interface IItemUpgradeView : IView
    {
        event Action<Item> ItemPlaced;
        event Action<Item> ItemRemoved;
        event Action UpgradeButtonClicked;

        void Construct(InventoryPanel inventoryPanel, InventoryComponent characterInventory, InventoryComponent ingredientInventory);
        void ChangeTitle(string title);
        void Refresh(Item item, List<RecipeIngredient> ingredients);
        void RefreshCost([CanBeNull] Currency cost);
        void Cleanup();
    }
}