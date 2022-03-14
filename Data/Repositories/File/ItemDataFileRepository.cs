using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    internal class ItemDataMapper : FakeMapper<ItemData> {}

    public class ItemDataFileRepository : FileRepository<int, ItemData, ItemData>, IItemDataRepository
    {
        public ItemDataFileRepository(IFileReader reader) : base(reader, new ItemDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/items.json";
        }
    }
}