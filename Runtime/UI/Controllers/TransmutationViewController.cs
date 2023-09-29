using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Items.Transmutation.Recipes;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class TransmutationViewController : ViewController<ITransmutationView>
    {
        private const int c_MaxItems = 9;

        private readonly List<Item> m_Items = new();

        private readonly List<ITransmutationRecipe> m_TransmutationRecipes;

        private ITransmutationRecipe m_TransmutationRecipe;

        public TransmutationViewController(ITransmutationView view, IItemRepository itemRepository, IRarityRepository rarityRepository, IRecipeRepository recipeRepository) : base(view)
        {
            var alchemyRecipeFactory = new TransmutationRecipeFactory(Game.Instance.Character.Entity.GetComponent<InventoryComponent>(), itemRepository, rarityRepository);

            m_TransmutationRecipes = new List<ITransmutationRecipe>
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
                m_TransmutationRecipes.Add(new RegularTransmutationRecipe(recipe));
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

            for (var i = 0; i < c_MaxItems; i++)
            {
                m_Items.Add(Item.CreateEmpty());
            }

            View.Construct(Game.Instance.GetController<EquipmentViewController>().View.GetInventoryPanel(), m_Items, m_TransmutationRecipes);
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
            var index1 = m_Items.IndexOf(item1);
            var index2 = m_Items.IndexOf(item2);

            var temp = m_Items[index1];
            m_Items[index1] = m_Items[index2];
            m_Items[index2] = temp;

            View.RefreshItems(m_Items);
        }

        private void OnRemoveItem(Item item)
        {
            var index = m_Items.FindIndex(i => i == item);

            m_Items[index] = Item.CreateEmpty();

            View.Unblock(item);
            View.RefreshItems(m_Items);

            UpdatePossibleResult();
        }

        private void OnAddItem(Item item)
        {
            var index = m_Items.FindIndex(i => i.IsEmpty);

            if (index == -1)
            {
                return;
            }

            OnAddItemIndex(item, index);
        }

        private void OnAddItemIndex(Item item, int index)
        {
            var sameItemIndex = m_Items.FindIndex(i => i == item);

            if (sameItemIndex >= 0)
            {
                index = sameItemIndex;
            }

            View.Unblock(m_Items[index]);

            m_Items[index] = item;

            View.Block(item);
            View.RefreshItems(m_Items);

            UpdatePossibleResult();
        }

        private void OnCombine()
        {
            var item = m_TransmutationRecipe?.Combine(m_Items);

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
            foreach (var item in m_Items.Where(i => !i.IsEmpty))
            {
                View.Unblock(item);
            }

            m_Items.Clear();

            for (var i = 0; i < c_MaxItems; i++)
            {
                m_Items.Add(Item.CreateEmpty());
            }

            View.RefreshItems(m_Items);
        }

        private void UpdatePossibleResult()
        {
            if (m_Items.Count(item => !item.IsEmpty) == 0)
            {
                View.ClearResult();
                return;
            }

            m_TransmutationRecipe = m_TransmutationRecipes.FirstOrDefault(r => r.Match(m_Items));

            if (m_TransmutationRecipe == null)
            {
                View.ShowImpossibleCombination();
                return;
            }

            View.ShowResult(m_TransmutationRecipe.Name, m_TransmutationRecipe.Description);
        }
    }
}