using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary
{
    public static class CraftUtils
    {
        private static int GetSlotMultiplier(Item item)
        {
            return item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1;
        }

        private static float GetRarityIndex(Item item)
        {
            return (int) item.Rarity.Type;
        }

        public static List<Item> GetDismantleIngredients(Item item)
        {
            var repository = Container.Instance.Resolve<IItemRepository>();

            // Dismantling result is based on the recipe formula, but there is no case for 'Common' item
            // so let's just give quarter of the green item ingredients.
            if (item.Rarity.Type == RarityType.Common)
            {
                return new List<Item>
                {
                    repository.Find(Constants.c_ItemIdScrap).SetStack(3 * GetSlotMultiplier(item)),
                    repository.Find(Constants.c_ItemIdMagicDust).SetStack(1 * GetSlotMultiplier(item)),
                };
            }

            var scrap = 12 * (int) Mathf.Pow(5, GetRarityIndex(item) - 2) * GetSlotMultiplier(item) / 2;
            var dust = 4 * (int) Mathf.Pow(5, GetRarityIndex(item) - 2) * GetSlotMultiplier(item) / 2;
            var essence = 4 * (int) Mathf.Pow(5, GetRarityIndex(item) - 3) * GetSlotMultiplier(item) / 2;
            var crystal = 4 * (int) Mathf.Pow(5, GetRarityIndex(item) - 4) * GetSlotMultiplier(item) / 2;

            var ingredients = new List<Item>();

            if (scrap > 0)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdScrap).SetStack(scrap));
            }

            if (dust > 0)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdMagicDust).SetStack(dust));
            }

            if (essence > 0)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdGlowingEssence).SetStack(essence));
            }

            if (crystal > 0)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdShadowCrystal).SetStack(crystal));
            }

            if (item.Rarity.Type == RarityType.Unique)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdSphereOfTransmutation).SetStack(1));
            }

            if (item.Rarity.Type == RarityType.Legendary)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdSphereOfAugmentation).SetStack(1));
            }

            if (item.Rarity.Type == RarityType.Vision)
            {
                ingredients.Add(repository.Find(Constants.c_ItemIdSphereOfVisions).SetStack(1));
            }

            ingredients.AddRange(item.Sockets.Where(socket => !socket.IsEmpty));

            return ingredients;
        }

        public static List<Item> GetForgeIngredients(Item item)
        {
            var itemRepository = Container.Instance.Resolve<IItemRepository>();

            return new List<Item>()
            {
                itemRepository.Find(Constants.c_ItemIdScrap).Clone().SetStack(200 * GetSlotMultiplier(item)),
                itemRepository.Find(Constants.c_ItemIdMagicDust).Clone().SetStack(40 * GetSlotMultiplier(item)),
                itemRepository.Find(Constants.c_ItemIdGlowingEssence).Clone().SetStack(8 * GetSlotMultiplier(item)),
                itemRepository.Find(Constants.c_ItemIdShadowCrystal).Clone().SetStack(2 * GetSlotMultiplier(item)),
                itemRepository.Find(Constants.c_ItemIdSphereOfAugmentation).Clone().SetStack(1 * GetSlotMultiplier(item)),
            };
        }
    }
}