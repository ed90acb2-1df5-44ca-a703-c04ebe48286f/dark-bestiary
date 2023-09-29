using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateRandomUniqueGemTransmutationOperation : ITransmutationOperation
    {
        private readonly IItemRepository m_ItemRepository;

        public CreateRandomUniqueGemTransmutationOperation(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            return m_ItemRepository.Find(item =>
                    item.RarityId == Constants.c_ItemRarityIdUnique &&
                    item.TypeId == Constants.c_ItemTypeIdGem)
                .Random();
        }
    }
}