using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Managers;

namespace DarkBestiary.Items.Alchemy
{
    public class RegularAlchemyRecipe : IAlchemyRecipe
    {
        public string Name => Recipe.Item.ColoredName;
        public IReadOnlyCollection<IAlchemyIngredient> Ingredients { get; }
        public Recipe Recipe { get; }


        private readonly InventoryComponent inventory;
        private readonly CharacterManager characterManager;

        public RegularAlchemyRecipe(Recipe recipe, CharacterManager characterManager)
        {
            Recipe = recipe;
            Ingredients = recipe.Ingredients.Select(i => new ItemAlchemyIngredient(i.Item, i.Count)).ToList();

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