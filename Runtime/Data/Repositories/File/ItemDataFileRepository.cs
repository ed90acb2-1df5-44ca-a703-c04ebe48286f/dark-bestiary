using System;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;

namespace DarkBestiary.Data.Repositories.File
{
    internal class ItemDataMapper : FakeMapper<ItemData>
    {
    }

    public class ItemDataFileRepository : FileRepository<int, ItemData, ItemData>, IItemDataRepository
    {
        public ItemDataFileRepository(IFileReader reader) : base(reader, new ItemDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/items.json";
        }

        public ItemData? Random(Func<ItemData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Shuffle()
                .FirstOrDefault();
        }
    }
}