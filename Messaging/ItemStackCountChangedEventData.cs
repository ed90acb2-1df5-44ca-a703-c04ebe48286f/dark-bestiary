using DarkBestiary.Items;

namespace DarkBestiary.Messaging
{
    public struct ItemStackCountChangedEventData
    {
        public Item Item { get; }
        public int Index { get; }

        public ItemStackCountChangedEventData(Item item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}