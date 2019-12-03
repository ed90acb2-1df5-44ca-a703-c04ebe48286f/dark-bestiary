using System;
using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface IUnitDataRepository : IRepository<int, UnitData>
    {
        List<UnitData> Find(Func<UnitData, bool> predicate);
    }
}