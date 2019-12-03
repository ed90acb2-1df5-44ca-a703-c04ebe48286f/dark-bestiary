namespace DarkBestiary.Items.Alchemy
{
    public class ItemTypeAndRarityAlchemyIngredient : IAlchemyIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        private readonly ItemTypeType type;
        private readonly RarityType rarity;

        public ItemTypeAndRarityAlchemyIngredient(string name, string icon, ItemTypeType type, RarityType rarity, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;

            this.type = type;
            this.rarity = rarity;
        }

        public bool Match(Item item)
        {
            if (item.IsEmpty)
            {
                return false;
            }

            return item.Type.Type == this.type && item.Rarity.Type == this.rarity;
        }
    }
}