using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.UI.Views
{
    public interface ISphereCraftView : IView, IHideOnEscape
    {
        event Payload<Item> ItemPlaced;
        event Payload ItemRemoved;
        event Payload<Item> SphereChanged;
        event Payload CombineButtonClicked;

        void Construct(List<Item> spheres, InventoryPanel inventoryPanel);

        void ChangeItem(Item item);

        void ChangeSphereDescription(string title, string description);

        void UpdateSphereStackCount(InventoryComponent inventory);

        void OnSuccess();
    }
}