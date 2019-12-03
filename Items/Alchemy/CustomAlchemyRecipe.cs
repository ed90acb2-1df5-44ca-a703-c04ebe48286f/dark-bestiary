using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;

namespace DarkBestiary.Items.Alchemy
{
    public class CustomAlchemyRecipe : IAlchemyRecipe
    {
        public string Name { get; }
        public IReadOnlyCollection<IAlchemyIngredient> Ingredients { get; }

        private readonly InventoryComponent inventory;
        private readonly IAlchemyOperation operation;

        public CustomAlchemyRecipe(string name, InventoryComponent inventory, List<IAlchemyIngredient> ingredients, IAlchemyOperation operation)
        {
            Name = name;
            Ingredients = ingredients;

            this.inventory = inventory;
            this.operation = operation;
        }

        public bool Match(List<Item> items)
        {
            return Ingredients.Sum(i => i.Count) == items.Count(item => !item.IsEmpty) &&
                   Ingredients.Sum(i => i.Count) == FindIngredients(items).Count;
        }

        public Item Combine(List<Item> items)
        {
            var ingredients = FindIngredients(items);

            if (ingredients.Any(ingredient => !this.inventory.Contains(ingredient)))
            {
                throw new GameplayException($"Missing required ingredient.");
            }

            foreach (var ingredient in ingredients)
            {
                this.inventory.Remove(ingredient, 1);
            }

            var item = this.operation.Perform(items);

            if (!this.inventory.Contains(item))
            {
                this.inventory.PickupDoNotStack(item);
            }

            return item;
        }

        private List<Item> FindIngredients(List<Item> items)
        {
            var foundIngredients = new List<Item>();

            foreach (var ingredient in Ingredients)
            {
                for (var i = 0; i < ingredient.Count; i++)
                {
                    var matching = items.FirstOrDefault(
                        item => !foundIngredients.Contains(item) && ingredient.Match(item));

                    if (matching == null)
                    {
                        continue;
                    }

                    foundIngredients.Add(matching);
                }
            }

            return foundIngredients;
        }
    }
}