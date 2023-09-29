using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items.Transmutation.Ingredients;
using DarkBestiary.Items.Transmutation.Operations;

namespace DarkBestiary.Items.Transmutation.Recipes
{
    public class TransmutationRecipeFactory
    {
        private readonly InventoryComponent m_Inventory;
        private readonly IItemRepository m_ItemRepository;
        private readonly IRarityRepository m_RarityRepository;

        public TransmutationRecipeFactory(InventoryComponent inventory,
            IItemRepository itemRepository, IRarityRepository rarityRepository)
        {
            m_Inventory = inventory;
            m_ItemRepository = itemRepository;
            m_RarityRepository = rarityRepository;
        }

        public CustomTransmutationRecipe TransmuteMagicRune()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_magic_rune");
            const string icon = "Sprites/unidentified_item";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Magic, 1),
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Magic, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_magic_rune"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_magic_rune_description"),
                requirements,
                new CreateRandomRuneTransmutationOperation(m_ItemRepository, Constants.c_ItemRarityIdMagic));
        }

        public CustomTransmutationRecipe TransmuteRareRune()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_rare_rune");
            const string icon = "Sprites/unidentified_item";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Rare, 1),
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Rare, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_rare_rune"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_rare_rune_description"),
                requirements,
                new CreateRandomRuneTransmutationOperation(m_ItemRepository, Constants.c_ItemRarityIdRare));
        }

        public CustomTransmutationRecipe TransmuteUniqueRune()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_unique_rune");
            const string icon = "Sprites/unidentified_item";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Unique, 1),
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Unique, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_unique_rune"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_unique_rune_description"),
                requirements,
                new CreateRandomRuneTransmutationOperation(m_ItemRepository, Constants.c_ItemRarityIdUnique));
        }

        public CustomTransmutationRecipe TransmuteLegendaryRune()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_legendary_rune");
            const string icon = "Sprites/unidentified_item";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Legendary, 1),
                new ItemCategoryAndRarityTransmutationIngredient(name, icon, ItemCategory.s_Runes, RarityType.Legendary, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_legendary_rune"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_legendary_rune_description"),
                requirements,
                new CreateRandomRuneTransmutationOperation(m_ItemRepository, Constants.c_ItemRarityIdLegendary));
        }

        public CustomTransmutationRecipe TransmuteMagicGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_chipped_gem");
            const string icon = "Sprites/unidentified_gem_chipped";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Magic, 1),
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Magic, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_chipped_gem"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_chipped_gem_description"),
                requirements,
                new CreateRandomMagicGemTransmutationOperation(m_ItemRepository));
        }

        public CustomTransmutationRecipe TransmuteRareGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_regular_gem");
            const string icon = "Sprites/unidentified_gem";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Rare, 1),
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Rare, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_regular_gem"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_regular_gem_description"),
                requirements,
                new CreateRandomRareGemTransmutationOperation(m_ItemRepository));
        }

        public CustomTransmutationRecipe TransmuteUniqueGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_flawless_gem");
            const string icon = "Sprites/unidentified_gem_flawless";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Unique, 1),
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Unique, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_flawless_gem"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_flawless_gem_description"),
                requirements,
                new CreateRandomUniqueGemTransmutationOperation(m_ItemRepository));
        }

        public CustomTransmutationRecipe TransmuteLegendaryGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_perfect_gem");
            const string icon = "Sprites/unidentified_gem_perfect";

            var requirements = new List<ITransmutationIngredient>
            {
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Legendary, 1),
                new ItemTypeAndRarityTransmutationIngredient(name, icon, ItemTypeType.Gem, RarityType.Legendary, 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_perfect_gem"),
                I18N.Instance.Get("ui_alchemy_recipe_transmute_perfect_gem_description"),
                requirements,
                new CreateRandomLegendaryGemTransmutationOperation(m_ItemRepository));
        }

        public CustomTransmutationRecipe Create(string name, string description, List<ITransmutationIngredient> requirements, ITransmutationOperation operation)
        {
            return new CustomTransmutationRecipe(name, description, m_Inventory, requirements, operation);
        }
    }
}