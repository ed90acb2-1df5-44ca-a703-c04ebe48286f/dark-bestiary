using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateRandomRuneTransmutationOperation : ITransmutationOperation
    {
        private readonly IItemRepository itemRepository;
        private readonly int rarityId;

        public CreateRandomRuneTransmutationOperation(IItemRepository itemRepository, int rarityId)
        {
            this.itemRepository = itemRepository;
            this.rarityId = rarityId;
        }

        public Item Perform(List<Item> items)
        {
            return this.itemRepository.Find(item =>
                item.RarityId == this.rarityId && item.TypeId == Constants.ItemTypeIdRune
            ).Random();
        }
    }
}