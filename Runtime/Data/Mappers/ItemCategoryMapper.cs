using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemCategoryMapper : Mapper<ItemCategoryData, ItemCategory>
    {
        private readonly IItemTypeRepository m_ItemTypeRepository;

        public ItemCategoryMapper(IItemTypeRepository itemTypeRepository)
        {
            m_ItemTypeRepository = itemTypeRepository;
        }

        public override ItemCategoryData ToData(ItemCategory entity)
        {
            throw new System.NotImplementedException();
        }

        public override ItemCategory ToEntity(ItemCategoryData data)
        {
            var itemTypes = m_ItemTypeRepository.Find(data.ItemTypes);
            return new ItemCategory(data.Id, I18N.Instance.Get(data.NameKey), data.Type, itemTypes);
        }
    }
}