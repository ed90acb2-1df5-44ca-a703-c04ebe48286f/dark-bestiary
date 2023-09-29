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

        private readonly InventoryComponent m_Inventory;
        private readonly ITransmutationOperation m_Operation;

        public CustomTransmutationRecipe(string name, string description, InventoryComponent inventory, List<ITransmutationIngredient> ingredients, ITransmutationOperation operation)
        {
            Name = name;
            Description = description;
            Ingredients = ingredients;

            m_Inventory = inventory;
            m_Operation = operation;
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
                m_Inventory.Remove(item, count);
            }

            var result = m_Operation.Perform(items);

            if (!m_Inventory.Contains(result))
            {
                m_Inventory.PickupDoNotStack(result);
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