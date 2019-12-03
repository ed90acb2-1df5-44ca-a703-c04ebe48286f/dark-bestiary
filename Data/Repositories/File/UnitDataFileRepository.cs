using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class UnitDataFileRepository : FileRepository<int, UnitData, UnitData>, IUnitDataRepository
    {
        internal class UnitDataMapper : FakeMapper<UnitData> {}

        public UnitDataFileRepository(IFileReader loader) : base(loader, new UnitDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/units.json";
        }

        public List<UnitData> Find(Func<UnitData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}