using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items.Transmutation.Ingredients;
using DarkBestiary.Managers;

namespace DarkBestiary.Items.Transmutation.Recipes
{
    public class RegularTransmutationRecipe : ITransmutationRecipe
    {
        public string Name { get; }
        public string Description { get; }
        public IReadOnlyCollection<ITransmutationIngredient> Ingredients { get; }
        public Recipe Recipe { get; }

        private readonly InventoryComponent inventory;
        private readonly CharacterManager characterManager;

        public RegularTransmutationRecipe(Recipe recipe, CharacterManager characterManager)
        {
            Recipe = recipe;
            Ingredients = recipe.Ingredients.Select(i => new ItemTransmutationIngredient(i.Item, i.Count)).ToList();

            Name = Recipe.Item.ColoredName;
            Description = $"{I18N.Instance.Translate("ui_create")}: {Recipe.Item.BaseName.ToString()}";

            if (Recipe.Item.Type.Type == ItemTypeType.Rune)
            {
                Description += $" ({I18N.Instance.Translate("ui_rune")})";
            }

            this.inventory = characterManager.Character.Entity.GetComponent<InventoryComponent>();
            this.characterManager = characterManager;
        }

        public bool Match(List<Item> items)
        {
            return Recipe.Ingredients.All(ingredient => items.Any(item => item.Id == ingredient.Item.Id && item.StackCount >= ingredient.Count)) &&
                   Recipe.Ingredients.Count == items.Count(item => !item.IsEmpty);
        }

        public Item Combine(List<Item> items)
        {
            this.inventory.WithdrawIngredients(Recipe.Ingredients);

            this.characterManager.Character.UnlockRecipe(Recipe);

            var item = Recipe.Item.Clone();

            this.inventory.PickupDoNotStack(item);

            return item;
        }
    }
}