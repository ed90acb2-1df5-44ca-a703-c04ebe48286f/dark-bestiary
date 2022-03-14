using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class SetRarityTransmutationOperation : ITransmutationOperation
    {
        private readonly Rarity rarity;

        public SetRarityTransmutationOperation(Rarity rarity)
        {
            this.rarity = rarity;
        }

        public Item Perform(List<Item> items)
        {
            var item = items.First(i => i.IsDismantable);

            item.ChangeRarity(this.rarity);

            return item;
        }
    }
}