using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class SpecializationDataFileRepository : FileRepository<int, SpecializationData, SpecializationData>, ISpecializationDataRepository
    {
        internal class SpecializationDataMapper : FakeMapper<SpecializationData> {}

        public SpecializationDataFileRepository(IFileReader reader) : base(reader, new SpecializationDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/specializations.json";
        }
    }
}