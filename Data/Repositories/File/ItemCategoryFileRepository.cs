using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemCategoryFileRepository : FileRepository<int, ItemCategoryData, ItemCategory>, IItemCategoryRepository
    {
        public ItemCategoryFileRepository(IFileReader loader, ItemCategoryMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/item_categories.json";
        }

        public ItemCategory FindByType(ItemCategoryType type)
        {
            return LoadData().Where(data => data.Type == type).Select(this.Mapper.ToEntity).FirstOrDefault();
        }
    }
}