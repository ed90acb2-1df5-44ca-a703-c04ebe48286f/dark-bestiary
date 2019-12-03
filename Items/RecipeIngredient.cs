namespace DarkBestiary.Items
{
    public class RecipeIngredient
    {
        public Item Item { get; }
        public int Count { get; }

        public RecipeIngredient(Item item, int count)
        {
            Item = item;
            Count = count;
        }
    }
}