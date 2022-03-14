using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    internal class LootDataMapper : FakeMapper<LootData> {}

    public class LootDataFileRepository : FileRepository<int, LootData, LootData>, ILootDataRepository
    {
        public LootDataFileRepository(IFileReader reader) : base(reader, new LootDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/loot.json";
        }
    }
}