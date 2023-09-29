using DarkBestiary.Items;

namespace DarkBestiary.Events
{
    public struct ItemPickupEventData
    {
        public Item Item { get; }
        public int Index { get; }

        public ItemPickupEventData(Item item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}