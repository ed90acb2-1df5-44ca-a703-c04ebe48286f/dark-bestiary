using DarkBestiary.UI.Elements;

namespace DarkBestiary.Messaging
{
    public struct ItemDroppedEventData
    {
        public InventoryItem InventoryItem { get; }
        public InventoryItemSlot InventorySlot { get; }

        public ItemDroppedEventData(InventoryItem inventoryItem, InventoryItemSlot inventorySlot)
        {
            InventoryItem = inventoryItem;
            InventorySlot = inventorySlot;
        }
    }
}