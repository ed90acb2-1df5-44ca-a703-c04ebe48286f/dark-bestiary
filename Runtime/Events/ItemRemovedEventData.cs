using DarkBestiary.Items;

namespace DarkBestiary.Events
{
    public struct ItemRemovedEventData
    {
        public Item Item { get; }
        public Item Empty { get; }
        public int Index { get; }

        public ItemRemovedEventData(Item item, Item empty, int index)
        {
            Item = item;
            Empty = empty;
            Index = index;
        }
    }
}