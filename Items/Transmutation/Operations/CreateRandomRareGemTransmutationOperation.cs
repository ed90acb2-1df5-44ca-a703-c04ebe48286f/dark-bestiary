using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateRandomRareGemTransmutationOperation : ITransmutationOperation
    {
        private readonly IItemRepository itemRepository;

        public CreateRandomRareGemTransmutationOperation(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            return this.itemRepository.Find(item =>
                    item.RarityId == Constants.ItemRarityIdRare &&
                    item.TypeId == Constants.ItemTypeIdGem)
                .Random();
        }
    }
}