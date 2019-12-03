using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ArchetypeDataFileRepository : FileRepository<int, ArchetypeData, ArchetypeData>, IArchetypeDataRepository
    {
        internal class ArchetypeDataMapper : FakeMapper<ArchetypeData> {}

        public ArchetypeDataFileRepository(IFileReader loader) : base(loader, new ArchetypeDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/archetypes.json";
        }
    }
}