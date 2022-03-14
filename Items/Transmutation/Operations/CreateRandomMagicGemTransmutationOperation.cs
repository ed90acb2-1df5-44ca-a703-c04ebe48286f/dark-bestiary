using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateRandomMagicGemTransmutationOperation : ITransmutationOperation
    {
        private readonly IItemRepository itemRepository;

        public CreateRandomMagicGemTransmutationOperation(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            return this.itemRepository.Find(item =>
                    item.RarityId == Constants.ItemRarityIdMagic &&
                    item.TypeId == Constants.ItemTypeIdGem)
                .Random();
        }
    }
}