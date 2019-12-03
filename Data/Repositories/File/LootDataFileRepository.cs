using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    internal class LootDataMapper : FakeMapper<LootData> {}

    public class LootDataFileRepository : FileRepository<int, LootData, LootData>, ILootDataRepository
    {
        public LootDataFileRepository(IFileReader loader) : base(loader, new LootDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/loot.json";
        }
    }
}