using System.Collections.Generic;
using DarkBestiary.Data;

namespace DarkBestiary.Items
{
    public class Recipe
    {
        public int Id => this.data.Id;
        public RecipeCategory Category => this.data.Category;
        public Item Item { get; }
        public List<RecipeIngredient> Ingredients { get; }

        private readonly RecipeData data;

        public Recipe(RecipeData data, Item item, List<RecipeIngredient> ingredients)
        {
            this.data = data;

            Item = item;
            Item.SetStack(data.ItemCount);

            Ingredients = ingredients;
        }
    }
}