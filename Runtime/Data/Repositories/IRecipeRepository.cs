using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories
{
    public interface IRecipeRepository : IRepository<int, Recipe>
    {
        List<Recipe> Find(Func<RecipeData, bool> predicate = null);
    }
}