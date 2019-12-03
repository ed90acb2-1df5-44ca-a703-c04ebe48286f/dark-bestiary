using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class ItemCategoryMapper : Mapper<ItemCategoryData, ItemCategory>
    {
        private readonly IItemTypeRepository itemTypeRepository;

        public ItemCategoryMapper(IItemTypeRepository itemTypeRepository)
        {
            this.itemTypeRepository = itemTypeRepository;
        }

        public override ItemCategoryData ToData(ItemCategory entity)
        {
            throw new System.NotImplementedException();
        }

        public override ItemCategory ToEntity(ItemCategoryData data)
        {
            var itemTypes = this.itemTypeRepository.Find(data.ItemTypes);
            return new ItemCategory(data.Id, I18N.Instance.Get(data.NameKey), data.Type, itemTypes);
        }
    }
}