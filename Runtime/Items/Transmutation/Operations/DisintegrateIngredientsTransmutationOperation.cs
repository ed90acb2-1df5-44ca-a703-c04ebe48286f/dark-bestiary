using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items.Transmutation.Operations
{
    public class DisintegrateIngredientsTransmutationOperation : ITransmutationOperation
    {
        public const int c_RequiredStack = 5;

        private readonly IItemRepository m_ItemRepository;

        public DisintegrateIngredientsTransmutationOperation(IItemRepository itemRepository)
        {
            m_ItemRepository = itemRepository;
        }

        public Item Perform(List<Item> items)
        {
            var ingredient = items.First(i => i.Type?.Type == ItemTypeType.Ingredient);

            if (ingredient.Id == Constants.c_ItemIdSphereOfAscension)
            {
                return m_ItemRepository.Find(Constants.c_ItemIdSphereOfAugmentation).SetStack(Rng.Range(1 * c_RequiredStack, 2 * c_RequiredStack));
            }

            if (ingredient.Id == Constants.c_ItemIdShadowCrystal)
            {
                return m_ItemRepository.Find(Constants.c_ItemIdGlowingEssence).SetStack(Rng.Range(1 * c_RequiredStack, 2 * c_RequiredStack));
            }

            if (ingredient.Id == Constants.c_ItemIdGlowingEssence)
            {
                return m_ItemRepository.Find(Constants.c_ItemIdMagicDust).SetStack(Rng.Range(2 * c_RequiredStack, 3 * c_RequiredStack));
            }

            if (ingredient.Id == Constants.c_ItemIdMagicDust)
            {
                return m_ItemRepository.Find(Constants.c_ItemIdScrap).SetStack(Rng.Range(4 * c_RequiredStack, 5 * c_RequiredStack));
            }

            return null;
        }
    }
}