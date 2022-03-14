using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items.Transmutation.Ingredients;
using DarkBestiary.Items.Transmutation.Operations;

namespace DarkBestiary.Items.Transmutation.Recipes
{
    public class CustomTransmutationRecipe : ITransmutationRecipe
    {
        public string Name { get; }
        public string Description { get; }
        public IReadOnlyCollection<ITransmutationIngredient> Ingredients { get; }

        private readonly InventoryComponent inventory;
        private readonly ITransmutationOperation operation;

        public CustomTransmutationRecipe(string name, string description, InventoryComponent inventory, List<ITransmutationIngredient> ingredients, ITransmutationOperation operation)
        {
            Name = name;
            Description = description;
            Ingredients = ingredients;

            this.inventory = inventory;
            this.operation = operation;
        }

        public bool Match(List<Item> items)
        {
            if (Ingredients.Count != FindItemsToConsume(items).Count)
            {
                return false;
            }

            return items.Where(i => !i.IsEmpty).All(item =>
                {
                    var matchingIngredient = Ingredients.FirstOrDefault(ingredient => ingredient.Match(item));

                    if (matchingIngredient == null)
                    {
                        return false;
                    }

                    return item.StackCount >= matchingIngredient.Count;
                }
            );
        }

        public Item Combine(List<Item> items)
        {
            foreach (var (item, count) in FindItemsToConsume(items))
            {
                this.inventory.Remove(item, count);
            }

            var result = this.operation.Perform(items);

            if (!this.inventory.Contains(result))
            {
                this.inventory.PickupDoNotStack(result);
            }

            return result;
        }

        private List<Tuple<Item, int>> FindItemsToConsume(List<Item> items)
        {
            var itemsToConsume = new List<Tuple<Item, int>>();

            foreach (var ingredient in Ingredients)
            {
                for (var i = 0; i < ingredient.Count; i++)
                {
                    var matching = items.FirstOrDefault(
                        item => itemsToConsume.All(t => t.Item1 != item) && ingredient.Match(item));

                    if (matching == null)
                    {
                        continue;
                    }

                    itemsToConsume.Add(new Tuple<Item, int>(matching, ingredient.Count));
                }
            }

            return itemsToConsume;
        }
    }
}