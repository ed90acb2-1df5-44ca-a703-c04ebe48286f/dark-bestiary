namespace DarkBestiary.Items.Alchemy
{
    public class ItemWithSuffixAlchemyIngredient : IAlchemyIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        public ItemWithSuffixAlchemyIngredient(string name, string icon, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;
        }

        public bool Match(Item item)
        {
            return item.Flags.HasFlag(ItemFlags.HasRandomSuffix);
        }
    }
}