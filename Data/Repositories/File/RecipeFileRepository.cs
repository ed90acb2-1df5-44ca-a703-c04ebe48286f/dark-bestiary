using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class RecipeFileRepository : FileRepository<int, RecipeData, Recipe>, IRecipeRepository
    {
        public RecipeFileRepository(IFileReader loader, RecipeMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/recipes.json";
        }

        public List<Recipe> Find(Func<RecipeData, bool> predicate = null)
        {
            if (predicate == null)
            {
                predicate = _ => true;
            }

            return LoadData()
                .Where(predicate)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}