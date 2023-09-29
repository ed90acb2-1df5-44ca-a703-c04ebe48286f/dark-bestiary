using System;
using System.Collections.Generic;
using DarkBestiary.Talents;

namespace DarkBestiary.Data.Repositories
{
    public interface ITalentRepository : IRepository<int, Talent>
    {
        List<Talent> Find(Func<TalentData, bool> predicate);
    }
}