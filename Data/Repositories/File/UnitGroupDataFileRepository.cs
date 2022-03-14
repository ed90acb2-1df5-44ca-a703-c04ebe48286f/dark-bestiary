using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class UnitGroupDataFileRepository : FileRepository<int, UnitGroupData, UnitGroupData>, IUnitGroupDataRepository
    {
        internal class UnitGroupDataMapper : FakeMapper<UnitGroupData> {}

        public UnitGroupDataFileRepository(IFileReader reader) : base(reader, new UnitGroupDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/unit_groups.json";
        }
    }
}