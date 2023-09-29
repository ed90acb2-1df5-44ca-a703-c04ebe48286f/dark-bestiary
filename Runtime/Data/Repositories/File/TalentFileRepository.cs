using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Talents;

namespace DarkBestiary.Data.Repositories.File
{
    public class TalentFileRepository : FileRepository<int, TalentData, Talent>, ITalentRepository
    {
        public TalentFileRepository(IFileReader reader, TalentMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/talents.json";
        }

        public List<Talent> Find(Func<TalentData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }
    }
}