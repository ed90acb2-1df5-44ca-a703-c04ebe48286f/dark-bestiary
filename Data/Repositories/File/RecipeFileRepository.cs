using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class RecipeFileRepository : FileRepository<int, RecipeData, Recipe>, IRecipeRepository
    {
        public RecipeFileRepository(IFileReader reader, RecipeMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/recipes.json";
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