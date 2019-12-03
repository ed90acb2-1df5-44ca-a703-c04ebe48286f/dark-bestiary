using DarkBestiary.Items;

namespace DarkBestiary.Messaging
{
    public struct ItemsSwappedEventData
    {
        public Item Item1 { get; }
        public Item Item2 { get; }

        public ItemsSwappedEventData(Item item1, Item item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
    }
}