using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkillFileRepository : FileRepository<int, SkillData, Skill>, ISkillRepository
    {
        public SkillFileRepository(IFileReader reader, SkillMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/skills.json";
        }

        public List<Skill> Find(Func<SkillData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }

        public List<Skill> Learnable(Func<SkillData, bool>? predicate = null)
        {
            predicate ??= _ => true;

            return LoadData()
                .Learnable(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }
    }
}