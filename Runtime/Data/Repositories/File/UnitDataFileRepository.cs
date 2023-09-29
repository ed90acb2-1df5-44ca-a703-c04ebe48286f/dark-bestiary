using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class UnitDataFileRepository : FileRepository<int, UnitData, UnitData>, IUnitDataRepository
    {
        internal class UnitDataMapper : FakeMapper<UnitData> {}

        public UnitDataFileRepository(IFileReader reader) : base(reader, new UnitDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/units.json";
        }

        public List<UnitData> Find(Func<UnitData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }
    }
}