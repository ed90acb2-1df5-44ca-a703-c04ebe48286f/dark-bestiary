using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;

namespace DarkBestiary.Data.Repositories.File
{
    public class FoodFileRepository : FileRepository<int, FoodData, Food>, IFoodRepository
    {
        public FoodFileRepository(IFileReader reader, FoodMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/food.json";
        }

        public List<Food> Random(int count, Func<FoodData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Random(count)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}