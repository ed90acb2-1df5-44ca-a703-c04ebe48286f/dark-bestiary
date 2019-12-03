using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class FoodFileRepository : FileRepository<int, FoodData, Food>, IFoodRepository
    {
        public FoodFileRepository(IFileReader loader, FoodMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/food.json";
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