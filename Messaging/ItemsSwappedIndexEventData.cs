using DarkBestiary.Items;

namespace DarkBestiary.Messaging
{
    public struct ItemsSwappedIndexEventData
    {
        public Item Item1 { get; }
        public int Index1 { get; }
        public Item Item2 { get; }
        public int Index2 { get; }

        public ItemsSwappedIndexEventData(Item item1, int index1, Item item2, int index2)
        {
            Item1 = item1;
            Index1 = index1;
            Item2 = item2;
            Index2 = index2;
        }
    }
}