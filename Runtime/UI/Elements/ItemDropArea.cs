using System;
using DarkBestiary.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class ItemDropArea : MonoBehaviour, IDropHandler
    {
        public event Action<Item> ItemDroppedIn;

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