using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Items.Transmutation.Recipes;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TransmutationViewController : ViewController<ITransmutationView>
    {
        private const int MaxItems = 9;

        private readonly List<Item> items = new List<Item>();

        private readonly List<ITransmutationRecipe> transmutationRecipes;
        private readonly CharacterManager characterManager;

        private ITransmutationRecipe transmutationRecipe;

        public TransmutationViewController(ITransmutationView view, IItemRepository itemRepository,
            IRarityRepository rarityRepository, IRecipeRepository recipeRepository, CharacterManager characterManager) : base(view)
        {
            this.characterManager = characterManager;

            var alchemyRecipeFactory = new TransmutationRecipeFactory(
                characterManager.Character.Entity.GetComponent<InventoryComponent>(), itemRepository, rarityRepository);

            this.transmutationRecipes = new List<ITransmutationRecipe>
            {
                alchemyRecipeFactory.TransmuteMagicGem(),
                alchemyRecipeFactory.TransmuteRareGem(),
                alchemyRecipeFactory.TransmuteUniqueGem(),
                alchemyRecipeFactory.TransmuteLegendaryGem(),
                alchemyRecipeFactory.TransmuteMagicRune(),
                alchemyRecipeFactory.TransmuteRareRune(),
                alchemyRecipeFactory.TransmuteUniqueRune(),
                alchemyRecipeFactory.TransmuteLegendaryRune(),
            };

            foreach (var recipe in recipeRepository.Find(x => x.Category == RecipeCategory.Transmutation).OrderBy(x => x.Item.Rarity.Type).ThenBy(x => x.Item.Data.NameKey))
            {
                this.transmutationRecipes.Add(new RegularTransmutationRecipe(recipe, this.characterManager));
            }
        }

        protected override void OnInitialize()
        {
            View.AddItem += OnAddItem;
            View.AddItemIndex += OnAddItemIndex;
            View.RemoveItem += OnRemoveItem;
            View.SwapItems += OnSwapItems;
            View.Combine += OnCombine;
            View.Hidden += OnHidding;

            for (var i = 0; i < MaxItems; i++)
            {
                this.items.Add(Item.CreateEmpty());
            }

            View.Construct(ViewControllerRegistry.Get<EquipmentViewController>().View.GetInventoryPanel(), this.items, this.transmutationRecipes);
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
            var item = this.transmutationRecipe?.Combine(this.items);

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
            if (this.items.Count(item => !item.IsEmpty) == 0)
            {
                View.ClearResult();
                return;
            }

            this.transmutationRecipe = this.transmutationRecipes.FirstOrDefault(r => r.Match(this.items));

            if (this.transmutationRecipe == null)
            {
                View.ShowImpossibleCombination();
                return;
            }

            if (this.transmutationRecipe is RegularTransmutationRecipe regularAlchemyRecipe)
            {
                if (this.characterManager.Character.Data.UnlockedRecipes.Any(id => regularAlchemyRecipe.Recipe.Id == id))
                {
                    View.ShowResult(regularAlchemyRecipe.Recipe);
                    return;
                }
            }

            View.ShowResult(this.transmutationRecipe.Name, this.transmutationRecipe.Description);
        }
    }
}