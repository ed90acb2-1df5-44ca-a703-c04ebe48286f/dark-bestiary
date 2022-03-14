namespace DarkBestiary.Items.Transmutation.Ingredients
{
    public interface ITransmutationIngredient
    {
        string Name { get; }
        string Icon { get; }
        int Count { get; }

        bool Match(Item item);
    }
}