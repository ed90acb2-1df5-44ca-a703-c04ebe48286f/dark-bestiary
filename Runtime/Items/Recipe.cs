using System.Collections.Generic;
using DarkBestiary.Data;

namespace DarkBestiary.Items
{
    public class Recipe
    {
        public int Id => m_Data.Id;
        public RecipeCategory Category => m_Data.Category;
        public Item Item { get; }
        public List<RecipeIngredient> Ingredients { get; }

        private readonly RecipeData m_Data;

        public Recipe(RecipeData data, Item item, List<RecipeIngredient> ingredients)
        {
            m_Data = data;

            Item = item;
            Item.SetStack(data.ItemCount);

            Ingredients = ingredients;
        }
    }
}