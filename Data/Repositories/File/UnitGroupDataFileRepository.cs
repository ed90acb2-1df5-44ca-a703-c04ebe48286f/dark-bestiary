using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class UnitGroupDataFileRepository : FileRepository<int, UnitGroupData, UnitGroupData>, IUnitGroupDataRepository
    {
        internal class UnitGroupDataMapper : FakeMapper<UnitGroupData> {}

        public UnitGroupDataFileRepository(IFileReader loader) : base(loader, new UnitGroupDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/unit_groups.json";
        }
    }
}