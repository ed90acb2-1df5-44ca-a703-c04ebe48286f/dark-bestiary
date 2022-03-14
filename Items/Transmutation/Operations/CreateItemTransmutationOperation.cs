using System.Collections.Generic;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateItemTransmutationOperation : ITransmutationOperation
    {
        private readonly Item item;

        public CreateItemTransmutationOperation(Item item)
        {
            this.item = item;
        }

        public Item Perform(List<Item> items)
        {
            return this.item.Clone();
        }
    }
}