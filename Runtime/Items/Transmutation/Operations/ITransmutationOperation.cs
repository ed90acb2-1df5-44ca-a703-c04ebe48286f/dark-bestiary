using System.Collections.Generic;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public interface ITransmutationOperation
    {
        Item Perform(List<Item> items);
    }
}