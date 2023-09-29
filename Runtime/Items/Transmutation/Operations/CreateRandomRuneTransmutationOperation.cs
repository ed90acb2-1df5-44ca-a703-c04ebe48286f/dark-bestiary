using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class CreateRandomRuneTransmutationOperation : ITransmutationOperation
    {
        private readonly IItemRepository m_ItemRepository;
        private readonly int m_RarityId;

        public CreateRandomRuneTransmutationOperation(IItemRepository itemRepository, int rarityId)
        {
            m_ItemRepository = itemRepository;
            m_RarityId = rarityId;
        }

        public Item Perform(List<Item> items)
        {
            return m_ItemRepository.Find(
                    item => item.RarityId == m_RarityId &&
                            item.TypeId == Constants.c_ItemTypeIdRune ||
                            item.TypeId == Constants.c_ItemTypeIdMinorRune ||
                            item.TypeId == Constants.c_ItemTypeIdMajorRune)
                .Random();
        }
    }
}