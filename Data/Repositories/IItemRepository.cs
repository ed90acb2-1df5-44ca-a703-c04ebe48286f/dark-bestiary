using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories
{
    public interface IItemRepository : IRepository<int, Item>
    {
        List<Item> Find(Func<ItemData, bool> predicate);

        List<Item> Random(int count, Func<ItemData, bool> predicate);

        List<Item> FindGambleable();
    }
}