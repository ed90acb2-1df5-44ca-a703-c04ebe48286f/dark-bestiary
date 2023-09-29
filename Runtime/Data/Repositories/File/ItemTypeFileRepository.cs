using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemTypeFileRepository : FileRepository<int, ItemTypeData, ItemType>, IItemTypeRepository
    {
        public ItemTypeFileRepository(IFileReader reader, ItemTypeMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/item_types.json";
        }
    }
}