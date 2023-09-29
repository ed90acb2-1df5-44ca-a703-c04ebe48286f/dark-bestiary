using System;

namespace DarkBestiary.Data.Repositories
{
    public interface IItemDataRepository : IRepository<int, ItemData>
    {
        ItemData? Random(Func<ItemData, bool> predicate);
    }
}