using System.Collections.Generic;

namespace DarkBestiary.Items.Alchemy
{
    public interface IAlchemyOperation
    {
        Item Perform(List<Item> items);
    }
}