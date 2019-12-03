using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class EateryViewController : ViewController<IEateryView>
    {
        private readonly IFoodRepository foodRepository;
        private readonly CurrenciesComponent currencies;
        private readonly FoodComponent foodComponent;
        private readonly Currency price;

        private readonly Dictionary<FoodType, Food> foods = new Dictionary<FoodType, Food>();

        public EateryViewController(IEateryView view, IFoodRepository foodRepository,
            ICurrencyRepository currencyRepository, CharacterManager characterManager) : base(view)
        {
            this.foodRepository = foodRepository;
            this.price = currencyRepository.FindByType(CurrencyType.Gold);
            this.currencies = characterManager.Character.Entity.GetComponent<CurrenciesComponent>();
            this.foodComponent = characterManager.Character.Entity.GetComponent<FoodComponent>();

            foreach (var pair in this.foodComponent.Foods)
            {
                this.foods.Add(pair.Key, pair.Value);
            }
        }

        protected override void OnInitialize()
        {
            View.FoodSelected += OnFoodSelected;
            View.FoodRemoved += OnFoodRemoved;
            View.BuyButtonClicked += OnBuyButtonClicked;

            var assortment = new List<Food>();
            assortment.AddRange(this.foodRepository.Random(3, d => d.Type == FoodType.Entree));
            assortment.AddRange(this.foodRepository.Random(3, d => d.Type == FoodType.Dessert));
            assortment.AddRange(this.foodRepository.Random(3, d => d.Type == FoodType.Drink));

            View.Construct(this.foods, assortment);
            View.UpdatePrice(0, false);
        }

        protected override void OnTerminate()
        {
            View.FoodSelected -= OnFoodSelected;
            View.BuyButtonClicked -= OnBuyButtonClicked;
        }

        private void OnFoodSelected(Food food)
        {
            this.foods[food.Type] = food;

            OnFoodUpdated();
        }

        private void OnFoodRemoved(FoodType foodType)
        {
            this.foods[foodType] = null;
            OnFoodUpdated();
        }

        private void OnFoodUpdated()
        {
            this.price.Set(this.foods.Values.Where(f => f != null && !f.IsApplied).Sum(f => f.Price));

            View.UpdateSlots(this.foods);
            View.UpdatePrice(this.price.Amount, !this.currencies.HasEnough(this.price));
        }

        private void OnBuyButtonClicked()
        {
            try
            {
                this.currencies.Withdraw(this.price);
                this.foodComponent.Apply(this.foods);

                OnFoodUpdated();

                AudioManager.Instance.PlayItemBuy();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.Push(exception.Message);
            }
        }
    }
}