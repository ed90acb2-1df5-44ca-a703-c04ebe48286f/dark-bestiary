using System.Collections.Generic;

namespace DarkBestiary.Items.Alchemy
{
    public class CreateItemAlchemyOperation : IAlchemyOperation
    {
        private readonly Item item;

        public CreateItemAlchemyOperation(Item item)
        {
            this.item = item;
        }

        public Item Perform(List<Item> items)
        {
            return this.item.Clone();
        }
    }
}