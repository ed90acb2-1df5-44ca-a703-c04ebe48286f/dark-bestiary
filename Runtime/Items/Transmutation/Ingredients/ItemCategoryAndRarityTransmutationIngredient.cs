namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class ItemCategoryAndRarityTransmutationIngredient : ITransmutationIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly ItemCategory m_Category;
        private readonly RarityType m_Rarity;

        public ItemCategoryAndRarityTransmutationIngredient(string name, string icon, ItemCategory category, RarityType rarity, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;

            m_Category = category;
            m_Rarity = rarity;
        }

        public bool Match(Item item)
        {
            if (item.IsEmpty)
            {
                return false;
            }

            return m_Category.Contains(item.Type) && item.Rarity.Type == m_Rarity;
        }
    }
}