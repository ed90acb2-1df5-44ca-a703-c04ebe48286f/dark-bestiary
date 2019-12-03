using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Items.Alchemy
{
    public class SetRandomSuffixAlchemyOperation : IAlchemyOperation
    {
        public Item Perform(List<Item> items)
        {
            var item = items.First(i => i.Flags.HasFlag(ItemFlags.HasRandomSuffix));

            item.Suffix = null;
            item.RollSuffix();

            return item;
        }
    }
}