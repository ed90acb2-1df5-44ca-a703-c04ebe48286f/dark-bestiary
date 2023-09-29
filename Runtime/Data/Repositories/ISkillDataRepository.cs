using System;
using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface ISkillDataRepository : IRepository<int, SkillData>
    {
        List<SkillData> Find(Func<SkillData, bool> predicate);

        List<SkillData> Learnable(Func<SkillData, bool>? predicate = null);
    }
}