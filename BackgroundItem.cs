using DarkBestiary.Items;

namespace DarkBestiary
{
    public struct BackgroundItem
    {
        public Item Item { get; }
        public bool IsEquipped { get; }

        public BackgroundItem(Item item, bool isEquipped)
        {
            Item = item;
            IsEquipped = isEquipped;
        }
    }
}