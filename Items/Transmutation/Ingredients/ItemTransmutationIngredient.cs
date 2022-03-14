namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public class ItemTransmutationIngredient : ITransmutationIngredient
    {
        public string Name => this.item.ColoredName;
        public string Icon => this.item.Icon;
        public int Count { get; }

        private readonly Item item;

        public ItemTransmutationIngredient(Item item, int count)
        {
            this.item = item;
            Count = count;
        }

        public bool Match(Item item)
        {
            if (item.IsEmpty)
            {
                return false;
            }

            return item.Id == this.item.Id;
        }
    }
}