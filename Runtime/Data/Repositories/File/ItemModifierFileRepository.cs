using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Extensions;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemModifierFileRepository : FileRepository<int, ItemModifierData, ItemModifier>,
        IItemModifierRepository
    {
        private readonly IItemCategoryRepository m_ItemCategoryRepository;

        public ItemModifierFileRepository(IFileReader reader, ItemModifierMapper mapper,
            IItemCategoryRepository itemCategoryRepository) : base(reader, mapper)
        {
            m_ItemCategoryRepository = itemCategoryRepository;
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/item_modifiers.json";
        }

        public List<ItemModifier> Find(Func<ItemModifierData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(Mapper.ToEntity)
                .ToList();
        }

        public ItemModifier RandomSuffix()
        {
            return Mapper.ToEntity(LoadData().Where(m => m.IsSuffix).Random());
        }

        public ItemModifier RandomSuffixForItem(Item item)
        {
            var modifier = LoadData()
                .Where(m => m.IsSuffix)
                .Where(m => m.Categories.Count == 0 || m.Categories.Any(c => item.Type.Categories.Contains(c)))
                .Random();

            return Mapper.ToEntity(modifier);
        }
    }
}