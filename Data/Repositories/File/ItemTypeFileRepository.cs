using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemTypeFileRepository : FileRepository<int, ItemTypeData, ItemType>, IItemTypeRepository
    {
        public ItemTypeFileRepository(IFileReader loader, ItemTypeMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/item_types.json";
        }
    }
}