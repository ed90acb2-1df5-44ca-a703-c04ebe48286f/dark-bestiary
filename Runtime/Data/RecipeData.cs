using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class RecipeData : Identity<int>
    {
        public int ItemId;
        public int ItemCount;
        public RecipeCategory Category;
        public List<RecipeIngredientData> Ingredients;
    }
}