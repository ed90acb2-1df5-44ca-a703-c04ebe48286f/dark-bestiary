using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateRandomLegendaryGemTransmutationOperation : ITransmutationOperation
    {
        private readonly IItemRepository itemRepository;

        public CreateRandomLegendaryGemTransmutationOperation(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            return this.itemRepository.Find(item =>
                    item.RarityId == Constants.ItemRarityIdLegendary &&
                    item.TypeId == Constants.ItemTypeIdGem)
                .Random();
        }
    }
}