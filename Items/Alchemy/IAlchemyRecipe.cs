using System.Collections.Generic;

namespace DarkBestiary.Items.Alchemy
{
    public interface IAlchemyRecipe
    {
        string Name { get; }
        IReadOnlyCollection<IAlchemyIngredient> Ingredients { get; }

        bool Match(List<Item> items);

        Item Combine(List<Item> items);
    }
}