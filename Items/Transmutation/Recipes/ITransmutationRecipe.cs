using System.Collections.Generic;
using DarkBestiary.Items.Transmutation.Ingredients;

namespace DarkBestiary.Items.Transmutation.Recipes
{
    public interface ITransmutationRecipe
    {
        string Name { get; }
        string Description { get; }

        IReadOnlyCollection<ITransmutationIngredient> Ingredients { get; }

        bool Match(List<Item> items);

        Item Combine(List<Item> items);
    }
}