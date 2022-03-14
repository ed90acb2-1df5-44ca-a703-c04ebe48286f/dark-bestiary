using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary
{
    public static class CraftUtils
    {
        private const int DefaultCraftRecipeScrapCount = 6;
        private const int DefaultCraftRecipeDustCount = 2;

        public static readonly Dictionary<int, float> SharpeningTable = new Dictionary<int, float>()
        {
             {1, 0.9f},
             {2, 0.9f},
             {3, 0.8f},
             {4, 0.8f},
             {5, 0.6f},
             {6, 0.6f},
             {7, 0.5f},
             {8, 0.5f},
             {9, 0.4f},
            {10, 0.4f},
            {11, 0.3f},
            {12, 0.3f},
            {13, 0.2f},
            {14, 0.2f},
            {15, 0.1f},
        };

        public static float GetRarityMultiplier(Item item)
        {
            var fraction = 1.0f;

            if (item.IsTwoHandedWeapon || item.IsChestArmor)
            {
                fraction = 2.0f;
            }

            var rarityIndex = (int) item.Rarity.Type;

            return fraction + Mathf.Min(rarityIndex - 2, 5) * 0.25f;
        }

        public static List<Item> GetForgeIngredients(Item item)
        {
            var result = new List<Item>();

            if (item.Level <= 1)
            {
                return result;
            }

            var itemRepository = Container.Instance.Resolve<IItemRepository>();

            AddToResult(result, itemRepository.Find(Constants.ItemIdScrap), item.Level * 5);
            AddToResult(result, itemRepository.Find(Constants.ItemIdMagicDust), item.Level);

            if (item.Rarity.Type >= RarityType.Rare)
            {
                AddToResult(result, itemRepository.Find(Constants.ItemIdGlowingEssence),
                    (int) Math.Ceiling(item.Level * 0.5f * GetRarityMultiplier(item)));
            }

            if (item.Rarity.Type >= RarityType.Unique)
            {
                AddToResult(result, itemRepository.Find(Constants.ItemIdShadowCrystal),
                    (int) Math.Ceiling(item.Level * 0.5f * GetRarityMultiplier(item)));
            }

            AddToResult(result, itemRepository.Find(Constants.ItemIdCatalyst),
                item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1);

            AddToResult(result, itemRepository.Find(Constants.ItemIdStabilizer),
                item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1);

            AddToResult(result, itemRepository.Find(Constants.ItemIdSphereOfAugmentation),
                item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1);

            return result;
        }

        public static List<Item> RollDismantleIngredients(Item item)
        {
            var repository = Container.Instance.Resolve<IItemRepository>();

            var ingredients = new List<Item>
            {
                repository.Find(Constants.ItemIdScrap)
                    .SetStack((int) (DefaultCraftRecipeScrapCount * GetRarityMultiplier(item))),
            };

            if (item.Rarity.Type >= RarityType.Magic)
            {
                ingredients.Add(repository.Find(Constants.ItemIdMagicDust)
                    .SetStack((int) (DefaultCraftRecipeDustCount * GetRarityMultiplier(item))));
            }

            if (item.Rarity.Type >= RarityType.Rare)
            {
                ingredients.Add(repository.Find(Constants.ItemIdGlowingEssence).SetStack(RNG.Range(1, 2)));
            }

            if (item.Rarity.Type >= RarityType.Unique)
            {
                ingredients.Add(repository.Find(Constants.ItemIdGlowingEssence).SetStack(RNG.Range(2, 4)));
                ingredients.Add(repository.Find(Constants.ItemIdShadowCrystal).SetStack(1));
            }

            if (item.Rarity.Type >= RarityType.Legendary)
            {
                ingredients.Add(repository.Find(Constants.ItemIdGlowingEssence).SetStack(RNG.Range(4, 6)));
                ingredients.Add(repository.Find(Constants.ItemIdShadowCrystal).SetStack(RNG.Range(1, 2)));
            }

            if (item.Rarity.Type == RarityType.Unique && RNG.Test(0.5f))
            {
                ingredients.Add(repository.Find(Constants.ItemIdSphereOfTransmutation).SetStack(1));
            }

            if (item.Rarity.Type == RarityType.Legendary && RNG.Test(0.5f))
            {
                ingredients.Add(repository.Find(Constants.ItemIdSphereOfAugmentation).SetStack(1));
            }

            if (item.Rarity.Type > RarityType.Legendary)
            {
                ingredients.Add(repository.Find(Constants.ItemIdSphereOfAugmentation).SetStack(2));
            }

            if (item.Rarity.Type == RarityType.Vision && RNG.Test(0.5f))
            {
                ingredients.Add(repository.Find(Constants.ItemIdSphereOfVisions).SetStack(1));
            }

            ingredients.AddRange(item.Sockets.Where(socket => !socket.IsEmpty));

            return ingredients;
        }

        private static void AddToResult(ICollection<Item> result, Item item, int count)
        {
            if (count <= item.StackCountMax)
            {
                result.Add(item.Clone().SetStack(count));
                return;
            }

            for (var i = 0; i < count / item.StackCountMax; i += item.StackCountMax)
            {
                result.Add(item.Clone().SetStack(item.StackCountMax));
            }

            var rest = count % item.StackCountMax;

            if (rest < 1)
            {
                return;
            }

            result.Add(item.Clone().SetStack(rest));
        }
    }
}