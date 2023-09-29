using System;
using System.Collections.Generic;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories
{
    public interface IItemModifierRepository : IRepository<int, ItemModifier>
    {
        List<ItemModifier> Find(Func<ItemModifierData, bool> predicate);

        ItemModifier RandomSuffix();

        ItemModifier RandomSuffixForItem(Item item);
    }
}