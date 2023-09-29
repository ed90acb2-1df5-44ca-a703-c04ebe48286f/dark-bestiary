namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class ItemTypeAndRarityTransmutationIngredient : ITransmutationIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly ItemTypeType m_Type;
        private readonly RarityType m_Rarity;

        public ItemTypeAndRarityTransmutationIngredient(string name, string icon, ItemTypeType type, RarityType rarity, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;

            m_Type = type;
            m_Rarity = rarity;
        }

        public bool Match(Item item)
        {
            if (item.IsEmpty)
            {
                return false;
            }

            return item.Type.Type == m_Type && item.Rarity.Type == m_Rarity;
        }
    }
}