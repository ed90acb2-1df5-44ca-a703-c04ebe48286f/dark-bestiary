using DarkBestiary.Items;
using DarkBestiary.Messaging;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class ItemDropArea : MonoBehaviour, IDropHandler
    {
        public event Payload<Item> ItemDroppedIn;

        public void OnDrop(PointerEventData pointer)
        {
            var inventoryItem = pointer.pointerDrag.GetComponent<InventoryItem>();

            if (inventoryItem == null)
            {
                return;
            }

            ItemDroppedIn?.Invoke(inventoryItem.Item);
        }
    }
}