using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items.Transmutation.Ingredients;

namespace DarkBestiary.Items.Transmutation.Recipes
{
    public class RegularTransmutationRecipe : ITransmutationRecipe
    {
        public string Name { get; }
        public string Description { get; }
        public IReadOnlyCollection<ITransmutationIngredient> Ingredients { get; }
        public Recipe Recipe { get; }

        private readonly InventoryComponent m_Inventory;

        public RegularTransmutationRecipe(Recipe recipe)
        {
            Recipe = recipe;
            Ingredients = recipe.Ingredients.Select(i => new ItemTransmutationIngredient(i.Item, i.Count)).ToList();

            Name = Recipe.Item.ColoredName;
            Description = $"{I18N.Instance.Translate("ui_create")}: {Recipe.Item.BaseName.ToString()}";

            if (Recipe.Item.Type.Type == ItemTypeType.Rune)
            {
                Description += $" ({I18N.Instance.Translate("ui_rune")})";
            }

            m_Inventory = Game.Instance.Character.Entity.GetComponent<InventoryComponent>();
        }

        public bool Match(List<Item> items)
        {
            return Recipe.Ingredients.All(ingredient => items.Any(item => item.Id == ingredient.Item.Id && item.StackCount >= ingredient.Count)) &&
                   Recipe.Ingredients.Count == items.Count(item => !item.IsEmpty);
        }

        public Item Combine(List<Item> items)
        {
            m_Inventory.WithdrawIngredients(Recipe.Ingredients);

            var item = Recipe.Item.Clone();

            m_Inventory.PickupDoNotStack(item);

            return item;
        }
    }
}