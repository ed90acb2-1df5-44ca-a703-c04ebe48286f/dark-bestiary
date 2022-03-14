using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;

namespace DarkBestiary.Data.Repositories
{
    public interface IBehaviourDataRepository : IRepository<int, BehaviourData>
    {
        List<BehaviourData> Find(Func<BehaviourData, bool> predicate);
    }
}