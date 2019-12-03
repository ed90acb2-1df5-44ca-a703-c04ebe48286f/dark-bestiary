using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories
{
    public interface IItemModifierRepository : IRepository<int, ItemModifier>
    {
        ItemModifier RandomSuffix();
    }
}