using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;

namespace DarkBestiary.Items.Alchemy
{
    public class CreateRandomMagicGemAlchemyOperation : IAlchemyOperation
    {
        private readonly IItemRepository itemRepository;

        public CreateRandomMagicGemAlchemyOperation(IItemRepository itemRepository)
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