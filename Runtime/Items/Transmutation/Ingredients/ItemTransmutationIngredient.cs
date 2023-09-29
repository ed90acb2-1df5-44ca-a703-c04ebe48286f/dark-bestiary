namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class ItemTransmutationIngredient : ITransmutationIngredient
    {
        public string Name => m_Item.ColoredName;
        public string Icon => m_Item.Icon;
        public int Count { get; }

        private readonly Item m_Item;

        public ItemTransmutationIngredient(Item item, int count)
        {
            m_Item = item;
            Count = count;
        }

        public bool Match(Item item)
        {
            if (item.IsEmpty)
            {
                return false;
            }

            return item.Id == m_Item.Id;
        }
    }
}