using System;
using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface IFoodRepository : IRepository<int, Food>
    {
        List<Food> Random(int count, Func<FoodData, bool> predicate);
    }
}