using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class ArchetypeDataFileRepository : FileRepository<int, ArchetypeData, ArchetypeData>, IArchetypeDataRepository
    {
        internal class ArchetypeDataMapper : FakeMapper<ArchetypeData> {}

        public ArchetypeDataFileRepository(IFileReader reader) : base(reader, new ArchetypeDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/archetypes.json";
        }
    }
}