using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Items.Alchemy;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class AlchemyViewController : ViewController<IAlchemyView>
    {
        private const int MaxItems = 9;

        private readonly List<Item> items = new List<Item>();

        private readonly List<IAlchemyRecipe> alchemyRecipes;
        private readonly CharacterManager characterManager;

        private IAlchemyRecipe alchemyRecipe;

        public AlchemyViewController(IAlchemyView view, IRecipeRepository recipeRepository, IItemRepository itemRepository, CharacterManager characterManager) : base(view)
        {
            this.characterManager = characterManager;

            this.alchemyRecipes = recipeRepository.Find(d => d.Category == RecipeCategory.Alchemy)
                .Select(recipe => new RegularAlchemyRecipe(recipe, this.characterManager) as IAlchemyRecipe)
                .ToList();

            var alchemyRecipeFactory = new AlchemyRecipeFactory(
                characterManager.Character.Entity.GetComponent<InventoryComponent>(), itemRepository);

            this.alchemyRecipes.Add(alchemyRecipeFactory.TransmuteEquipment());
            this.alchemyRecipes.Add(alchemyRecipeFactory.TransmuteRegularGem());
            this.alchemyRecipes.Add(alchemyRecipeFactory.TransmuteFlawlessGem());
            this.alchemyRecipes.Add(alchemyRecipeFactory.TransmutePerfectGem());
        }

        protected override void OnInitialize()
        {
            View.AddItem += OnAddItem;
            View.AddItemIndex += OnAddItemIndex;
            View.RemoveItem += OnRemoveItem;
            View.SwapItems += OnSwapItems;
            View.Combine += OnCombine;
            View.Hidding += OnHidding;

            for (var i = 0; i < MaxItems; i++)
            {
                this.items.Add(Item.CreateEmpty());
            }

            View.Construct(this.characterManager.Character, this.items, this.alchemyRecipes);
        }

        private void OnHidding()
        {
            Clear();
        }

        protected override void OnTerminate()
        {
            View.AddItem -= OnAddItem;
            View.AddItemIndex -= OnAddItemIndex;
            View.RemoveItem -= OnRemoveItem;
            View.SwapItems -= OnSwapItems;
            View.Combine -= OnCombine;
        }

        private void OnSwapItems(Item item1, Item item2)
        {
            var index1 = this.items.IndexOf(item1);
            var index2 = this.items.IndexOf(item2);

            var temp = this.items[index1];
            this.items[index1] = this.items[index2];
            this.items[index2] = temp;

            View.RefreshItems(this.items);
        }

        private void OnRemoveItem(Item item)
        {
            var index = this.items.FindIndex(i => i == item);

            this.items[index] = Item.CreateEmpty();

            View.Unblock(item);
            View.RefreshItems(this.items);

            UpdatePossibleResult();
        }

        private void OnAddItem(Item item)
        {
            var index = this.items.FindIndex(i => i.IsEmpty);

            if (index == -1)
            {
                return;
            }

            OnAddItemIndex(item, index);
        }

        private void OnAddItemIndex(Item item, int index)
        {
            var sameItemIndex = this.items.FindIndex(i => i == item);

            if (sameItemIndex >= 0)
            {
                index = sameItemIndex;
            }

            View.Unblock(this.items[index]);

            this.items[index] = item;

            View.Block(item);
            View.RefreshItems(this.items);

            UpdatePossibleResult();
        }

        private void OnCombine()
        {
            var item = this.alchemyRecipe?.Combine(this.items);

            Clear();

            if (item != null)
            {
                OnAddItemIndex(item, 4);
                View.OnSuccess();
            }

            UpdatePossibleResult();
        }

        private void Clear()
        {
            foreach (var item in this.items.Where(i => !i.IsEmpty))
            {
                View.Unblock(item);
            }

            this.items.Clear();

            for (var i = 0; i < MaxItems; i++)
            {
                this.items.Add(Item.CreateEmpty());
            }

            View.RefreshItems(this.items);
        }

        private void UpdatePossibleResult()
        {
            this.alchemyRecipe = this.alchemyRecipes.FirstOrDefault(r => r.Match(this.items));
            ShowPossibleResult();
        }

        private void ShowPossibleResult()
        {
            if (this.items.Count(item => !item.IsEmpty) == 0)
            {
                View.ClearResult();
                return;
            }

            if (this.alchemyRecipe == null)
            {
                View.ShowImpossibleCombination();
                return;
            }

            if (this.alchemyRecipe is RegularAlchemyRecipe regularAlchemyRecipe)
            {
                if (this.characterManager.Character.Data.UnlockedRecipes.Any(id => regularAlchemyRecipe.Recipe.Id == id))
                {
                    View.ShowResult(regularAlchemyRecipe.Recipe);
                    return;
                }
            }

            View.ShowUnknownResult();
        }
    }
}