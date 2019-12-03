namespace DarkBestiary.Items.Alchemy
{
    public interface IAlchemyIngredient
    {
        string Name { get; }
        string Icon { get; }
        int Count { get; }

        bool Match(Item item);
    }
}