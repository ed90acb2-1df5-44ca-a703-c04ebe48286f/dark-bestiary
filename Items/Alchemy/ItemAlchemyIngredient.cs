namespace DarkBestiary.Items.Alchemy
{
    public class ItemAlchemyIngredient : IAlchemyIngredient
    {
        public string Name => this.item.ColoredName;
        public string Icon => this.item.Icon;
        public int Count { get; }

        private readonly Item item;

        public ItemAlchemyIngredient(Item item, int count)
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