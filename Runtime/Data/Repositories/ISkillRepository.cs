using System;
using System.Collections.Generic;
using DarkBestiary.Skills;

namespace DarkBestiary.Data.Repositories
{
    public interface ISkillRepository : IRepository<int, Skill>
    {
        List<Skill> Find(Func<SkillData, bool> predicate);

        List<Skill> Learnable(Func<SkillData, bool> predicate = null);
    }
}