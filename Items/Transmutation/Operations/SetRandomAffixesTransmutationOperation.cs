using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Behaviours;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class SetRandomAffixesTransmutationOperation : ITransmutationOperation
    {
        public Item Perform(List<Item> items)
        {
            var item = items.First(i => i.Flags.HasFlag(ItemFlags.HasRandomAffixes));

            item.Affixes = new List<Behaviour>();
            item.RollAffixes();

            return item;
        }
    }
}