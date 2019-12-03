using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories
{
    public interface IItemCategoryRepository : IRepository<int, ItemCategory>
    {
        ItemCategory FindByType(ItemCategoryType type);
    }
}