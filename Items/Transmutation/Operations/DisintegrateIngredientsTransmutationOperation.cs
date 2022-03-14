using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class DisintegrateIngredientsTransmutationOperation : ITransmutationOperation
    {
        public const int RequiredStack = 5;

        private readonly IItemRepository itemRepository;

        public DisintegrateIngredientsTransmutationOperation(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            var ingredient = items.First(i => i.Type?.Type == ItemTypeType.Ingredient);

            if (ingredient.Id == Constants.ItemIdSphereOfAscension)
            {
                return this.itemRepository.Find(Constants.ItemIdSphereOfAugmentation).SetStack(RNG.Range(1 * RequiredStack, 2 * RequiredStack));
            }

            if (ingredient.Id == Constants.ItemIdShadowCrystal)
            {
                return this.itemRepository.Find(Constants.ItemIdGlowingEssence).SetStack(RNG.Range(1 * RequiredStack, 2 * RequiredStack));
            }

            if (ingredient.Id == Constants.ItemIdGlowingEssence)
            {
                return this.itemRepository.Find(Constants.ItemIdMagicDust).SetStack(RNG.Range(2 * RequiredStack, 3 * RequiredStack));
            }

            if (ingredient.Id == Constants.ItemIdMagicDust)
            {
                return this.itemRepository.Find(Constants.ItemIdScrap).SetStack(RNG.Range(4 * RequiredStack, 5 * RequiredStack));
            }

            return null;
        }
    }
}