using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemModifierFileRepository : FileRepository<int, ItemModifierData, ItemModifier>,
        IItemModifierRepository
    {
        private readonly IItemCategoryRepository itemCategoryRepository;

        public ItemModifierFileRepository(IFileReader loader, ItemModifierMapper mapper,
            IItemCategoryRepository itemCategoryRepository) : base(loader, mapper)
        {
            this.itemCategoryRepository = itemCategoryRepository;
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/item_modifiers.json";
        }

        public ItemModifier RandomSuffix()
        {
            return this.Mapper.ToEntity(LoadData().Where(m => m.IsSuffix).Random());
        }
    }
}