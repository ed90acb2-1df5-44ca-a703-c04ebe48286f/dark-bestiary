using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemSetFileRepository : FileRepository<int, ItemSetData, ItemSet>, IItemSetRepository
    {
        public ItemSetFileRepository(IFileReader reader, ItemSetMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/item_sets.json";
        }
    }
}