using System;
using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Mappers
{
    public class RecipeMapper : Mapper<RecipeData, Recipe>
    {
        private readonly IItemRepository itemRepository;

        public RecipeMapper(IItemRepository itemRepository)
        {
            this.itemRepository = itemRepository;
        }

        public override RecipeData ToData(Recipe target)
        {
            throw new NotImplementedException();
        }

        public override Recipe ToEntity(RecipeData data)
        {
            var item = this.itemRepository.FindOrFail(data.ItemId);
            var ingredients = new List<RecipeIngredient>();

            foreach (var ingredient in data.Ingredients)
            {
                ingredients.Add(new RecipeIngredient(this.itemRepository.FindOrFail(ingredient.ItemId), ingredient.Count));
            }

            return new Recipe(data, item, ingredients);
        }
    }
}