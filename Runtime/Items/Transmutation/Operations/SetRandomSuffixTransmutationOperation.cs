using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class SetRandomSuffixTransmutationOperation : ITransmutationOperation
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