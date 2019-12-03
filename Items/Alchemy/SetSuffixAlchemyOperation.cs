using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items.Alchemy
{
    public class SetSuffixAlchemyOperation : IAlchemyOperation
    {
        private readonly IItemModifierRepository itemModifierRepository;
        private readonly int suffixId;

        public SetSuffixAlchemyOperation(int suffixId, IItemModifierRepository itemModifierRepository)
        {
            this.suffixId = suffixId;
            this.itemModifierRepository = itemModifierRepository;
        }

        public Item Perform(List<Item> items)
        {
            var suffix = this.itemModifierRepository.Find(this.suffixId);

            if (!suffix.IsSuffix)
            {
                throw new Exception($"ItemModifier with id {this.suffixId} is not a suffix.");
            }

            var item = items.First(i => i.Flags.HasFlag(ItemFlags.HasRandomSuffix));

            item.ChangeSuffix(suffix);

            return item;
        }
    }
}