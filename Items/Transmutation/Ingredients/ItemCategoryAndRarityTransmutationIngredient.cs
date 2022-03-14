namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class ItemCategoryAndRarityTransmutationIngredient : ITransmutationIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly ItemCategory category;
        private readonly RarityType rarity;

        public ItemCategoryAndRarityTransmutationIngredient(string name, string icon, ItemCategory category, RarityType rarity, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;

            this.category = category;
            this.rarity = rarity;
        }

        public bool Match(Item item)
        {
            if (item.IsEmpty)
            {
                return false;
            }

            return this.category.Contains(item.Type) && item.Rarity.Type == this.rarity;
        }
    }
}