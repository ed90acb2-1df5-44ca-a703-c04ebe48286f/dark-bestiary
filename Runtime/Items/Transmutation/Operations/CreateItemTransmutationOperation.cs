using System.Collections.Generic;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateItemTransmutationOperation : ITransmutationOperation
    {
        private readonly Item m_Item;

        public CreateItemTransmutationOperation(Item item)
        {
            m_Item = item;
        }

        public Item Perform(List<Item> items)
        {
            return m_Item.Clone();
        }
    }
}