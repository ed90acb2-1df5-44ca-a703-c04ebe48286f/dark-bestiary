using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class SetRarityTransmutationOperation : ITransmutationOperation
    {
        private readonly Rarity m_Rarity;

        public SetRarityTransmutationOperation(Rarity rarity)
        {
            m_Rarity = rarity;
        }

        public Item Perform(List<Item> items)
        {
            var item = items.First(i => i.IsDismantable);

            item.ChangeRarity(m_Rarity);

            return item;
        }
    }
}