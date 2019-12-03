namespace DarkBestiary.Items.Alchemy
{
    public class AnyRareOrHigherDismantableItemAlchemyIngredient : IAlchemyIngredient
    {
        public string Name { get; }
        public string Icon { get; }
        public int Count { get; }

        public AnyRareOrHigherDismantableItemAlchemyIngredient(string name, string icon, int count)
        {
            Name = name;
            Icon = icon;
            Count = count;
        }

        public bool Match(Item item)
        {
            return !item.IsEmpty && item.IsDismantable && (int )item.Rarity.Type > 2;
        }
    }
}