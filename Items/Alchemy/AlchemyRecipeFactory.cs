using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Items.Alchemy
{
    public class AlchemyRecipeFactory
    {
        private readonly InventoryComponent inventory;
        private readonly IItemRepository itemRepository;

        public AlchemyRecipeFactory(InventoryComponent inventory, IItemRepository itemRepository)
        {
            this.inventory = inventory;
            this.itemRepository = itemRepository;
        }

        public CustomAlchemyRecipe TransmuteEquipment()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_any_magic_item");
            const string icon = "Sprites/unidentified_item";

            var requirements = new List<IAlchemyIngredient>
            {
                new ItemWithSuffixAlchemyIngredient(name, icon, 1),
                new ItemAlchemyIngredient(this.itemRepository.Find(Constants.ItemIdSphereOfTransmutation), 1),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_equipment"),
                requirements,
                new SetRandomSuffixAlchemyOperation());
        }

        public CustomAlchemyRecipe TransmuteRegularGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_regular_gem");
            const string icon = "Sprites/unidentified_gem";

            var requirements = new List<IAlchemyIngredient>
            {
                new ItemTypeAndRarityAlchemyIngredient(name, icon, ItemTypeType.Gem, RarityType.Magic, 2),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_regular_gem"),
                requirements,
                new CreateRandomMagicGemAlchemyOperation(this.itemRepository));
        }

        public CustomAlchemyRecipe TransmuteFlawlessGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_flawless_gem");
            const string icon = "Sprites/unidentified_gem_flawless";

            var requirements = new List<IAlchemyIngredient>
            {
                new ItemTypeAndRarityAlchemyIngredient(name, icon, ItemTypeType.Gem, RarityType.Rare, 2),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_flawless_gem"),
                requirements,
                new CreateRandomRareGemAlchemyOperation(this.itemRepository));
        }

        public CustomAlchemyRecipe TransmutePerfectGem()
        {
            var name = I18N.Instance.Get("ui_alchemy_ingredient_perfect_gem");
            const string icon = "Sprites/unidentified_gem_perfect";

            var requirements = new List<IAlchemyIngredient>
            {
                new ItemTypeAndRarityAlchemyIngredient(name, icon, ItemTypeType.Gem, RarityType.Unique, 2),
            };

            return Create(
                I18N.Instance.Get("ui_alchemy_recipe_transmute_perfect_gem"),
                requirements,
                new CreateRandomUniqueGemAlchemyOperation(this.itemRepository));
        }

        public CustomAlchemyRecipe Create(string name, List<IAlchemyIngredient> requirements, IAlchemyOperation operation)
        {
            return new CustomAlchemyRecipe(name, this.inventory, requirements, operation);
        }
    }
}