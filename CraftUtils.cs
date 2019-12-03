using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary
{
    public static class CraftUtils
    {
        private const int DefaultCraftRecipeScrapCount = 6;
        private const int DefaultCraftRecipeDustCount = 2;

        public static float GetQualityMultiplier(Item item)
        {
            var fraction = 1.0f;

            if (item.IsTwoHandedWeapon || item.IsChestArmor)
            {
                fraction = 2.0f;
            }

            return fraction + ((int) item.Rarity.Type - 2) * 0.25f;
        }

        private static List<Item> GetCraftIngredients(Item item)
        {
            var itemRepository = Container.Instance.Resolve<IItemRepository>();

            var ingredients = new List<Item>
            {
                itemRepository.Find(Constants.ItemIdScrap)
                    .SetStack((int) (DefaultCraftRecipeScrapCount * GetQualityMultiplier(item))),
            };

            if ((int) item.Rarity.Type > 1)
            {
                ingredients.Add(itemRepository.Find(Constants.ItemIdMagicDust)
                    .SetStack((int) (DefaultCraftRecipeDustCount * GetQualityMultiplier(item))));
            }

            if ((int) item.Rarity.Type > 2)
            {
                ingredients.Add(itemRepository.Find(Constants.ItemIdGlowingEssence).SetStack(1));
            }

            if ((int) item.Rarity.Type > 3)
            {
                ingredients.Add(itemRepository.Find(Constants.ItemIdShadowCrystal).SetStack(1));
            }

            if (item.Rarity.Type == RarityType.Unique)
            {
                ingredients.Add(itemRepository.Find(Constants.ItemIdSphereOfTransmutation).SetStack(1));
            }

            if (item.Rarity.Type == RarityType.Legendary)
            {
                ingredients.Add(itemRepository.Find(Constants.ItemIdSphereOfAugmentation).SetStack(1));
            }

            return ingredients;
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

            if ((int) item.Rarity.Type > 2)
            {
                AddToResult(result, itemRepository.Find(Constants.ItemIdGlowingEssence),
                    (int) Math.Ceiling(item.Level * 0.5f * GetQualityMultiplier(item)));
            }

            if ((int) item.Rarity.Type > 3)
            {
                AddToResult(result, itemRepository.Find(Constants.ItemIdShadowCrystal),
                    (int) Math.Ceiling(item.Level * 0.5f * GetQualityMultiplier(item)));
            }

            AddToResult(result, itemRepository.Find(Constants.ItemIdCatalyst),
                item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1);

            AddToResult(result, itemRepository.Find(Constants.ItemIdStabilizer),
                item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1);

            AddToResult(result, itemRepository.Find(Constants.ItemIdSphereOfAugmentation),
                item.IsTwoHandedWeapon || item.IsChestArmor ? 2 : 1);

            return result;
        }

        public static List<Item> GetDismantleIngredients(Item item)
        {
            var result = new List<Item>();
            result.AddRange(item.Sockets.Where(socket => !socket.IsEmpty));

            result.AddRange(
                GetCraftIngredients(item).Select(ingredient => ingredient.SetStack(ingredient.StackCount / 2)));

            result.AddRange(
                GetForgeIngredients(item)
                    .Where(ingredient => ingredient.Id != Constants.ItemIdCatalyst &&
                                         ingredient.Id != Constants.ItemIdStabilizer &&
                                         ingredient.Id != Constants.ItemIdSphereOfAugmentation)
                    .Select(ingredient => ingredient.SetStack(ingredient.StackCount / 2)));

            return result;
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