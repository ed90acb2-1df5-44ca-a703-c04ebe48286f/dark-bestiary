using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;

namespace DarkBestiary.Data.Repositories
{
    public interface IBehaviourRepository : IRepository<int, Behaviour>
    {
        List<Behaviour> Find(Func<BehaviourData, bool> predicate);

        List<Behaviour> Random(Func<BehaviourData, bool> predicate, int count);
    }
}