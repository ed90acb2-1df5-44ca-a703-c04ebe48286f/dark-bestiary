using System;
using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface ISphereCraftView : IView, IHideOnEscape
    {
        event Action<Item> ItemPlaced;
        event Action ItemRemoved;
        event Action<Item> SphereChanged;
        event Action CombineButtonClicked;

        void Construct(List<Item> spheres, InventoryPanel inventoryPanel);

        void ChangeItem(Item item);

        void ChangeSphereDescription(string title, string description);

        void UpdateSphereStackCount(InventoryComponent inventory);

        void OnSuccess();
    }
}