using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Alchemy
{
    public class CreateRandomUniqueGemAlchemyOperation : IAlchemyOperation
    {
        private readonly IItemRepository itemRepository;

        public CreateRandomUniqueGemAlchemyOperation(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            return this.itemRepository.Find(item =>
                    item.RarityId == Constants.ItemRarityIdUnique &&
                    item.TypeId == Constants.ItemTypeIdGem)
                .Random();
        }
    }
}