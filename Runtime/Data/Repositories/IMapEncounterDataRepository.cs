using System;
using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface IMapEncounterDataRepository : IRepository<int, MapEncounterData>
    {
        List<MapEncounterData> Find(Func<MapEncounterData, bool> predicate);
    }
}