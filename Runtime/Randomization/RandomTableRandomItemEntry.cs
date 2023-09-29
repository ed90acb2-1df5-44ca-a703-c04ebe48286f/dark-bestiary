using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Randomization
{
    public class RandomTableRandomItemEntry : RandomTableEntry
    {
        private readonly LootItemData m_Data;

        public RandomTableRandomItemEntry(LootItemData data, RandomTableEntryParameters parameters) : base(parameters)
        {
            m_Data = data;
        }

        public RandomTableItemEntry? Roll()
        {
            var item = Container.Instance.Resolve<IItemDataRepository>().Random(Filter);

            if (item == null)
            {
                return null;
            }

            return new RandomTableItemEntry(
                new RandomTableItemEntryValue(item.Id, Rng.Range(m_Data.StackMin, m_Data.StackMax)),
                new RandomTableEntryParameters(Weight, IsUnique, IsGuaranteed, IsEnabled)
            );

            bool Filter(ItemData itemData)
            {
                var result =
                    ItemDroppableFilter(itemData) &&
                    ItemCategoryFilter(itemData) &&
                    ItemRarityFilter(itemData);

                return result;
            }

            bool ItemDroppableFilter(ItemData itemData)
            {
                return itemData.IsEnabled && itemData.Flags.HasFlag(ItemFlags.Droppable);
            }

            bool ItemCategoryFilter(ItemData itemData)
            {
                if (m_Data.CategoryId == 0)
                {
                    return true;
                }

                return itemData.Categories.Contains(m_Data.CategoryId);
            }

            bool ItemRarityFilter(ItemData itemData)
            {
                return m_Data.RarityId == 0 || itemData.RarityId == m_Data.RarityId;
            }
        }
    }
}