using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;

namespace DarkBestiary.Data.Repositories.File
{
    internal class SkillDataMapper : FakeMapper<SkillData>
    {
    }

    public class SkillDataFileRepository : FileRepository<int, SkillData, SkillData>, ISkillDataRepository
    {
        public SkillDataFileRepository(IFileReader reader) : base(reader, new SkillDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/skills.json";
        }

        public List<SkillData> Find(Func<SkillData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }

        public List<SkillData> Learnable(Func<SkillData, bool>? predicate = null)
        {
            predicate ??= _ => true;
            return LoadData().Learnable(predicate).ToList();
        }
    }
}