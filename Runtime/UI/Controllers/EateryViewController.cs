using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class EateryViewController : ViewController<IEateryView>
    {
        private readonly IFoodRepository m_FoodRepository;
        private readonly CurrenciesComponent m_Currencies;
        private readonly FoodComponent m_FoodComponent;
        private readonly Currency m_Price;

        private readonly Dictionary<FoodType, Food> m_Foods = new();

        public EateryViewController(IEateryView view, IFoodRepository foodRepository, ICurrencyRepository currencyRepository) : base(view)
        {
            m_FoodRepository = foodRepository;
            m_Price = currencyRepository.FindByType(CurrencyType.Gold);
            m_Currencies = Game.Instance.Character.Entity.GetComponent<CurrenciesComponent>();
            m_FoodComponent = Game.Instance.Character.Entity.GetComponent<FoodComponent>();

            foreach (var pair in m_FoodComponent.Foods)
            {
                m_Foods.Add(pair.Key, pair.Value);
            }
        }

        protected override void OnInitialize()
        {
            View.FoodSelected += OnFoodSelected;
            View.FoodRemoved += OnFoodRemoved;
            View.BuyButtonClicked += OnBuyButtonClicked;

            var assortment = new List<Food>();
            assortment.AddRange(m_FoodRepository.Random(3, d => d.Type == FoodType.Entree));
            assortment.AddRange(m_FoodRepository.Random(3, d => d.Type == FoodType.Dessert));
            assortment.AddRange(m_FoodRepository.Random(3, d => d.Type == FoodType.Drink));

            View.Construct(m_Currencies, m_Foods, assortment);
            View.UpdatePrice(0, false);
        }

        protected override void OnTerminate()
        {
            View.FoodSelected -= OnFoodSelected;
            View.BuyButtonClicked -= OnBuyButtonClicked;
        }

        private void OnFoodSelected(Food food)
        {
            m_Foods[food.Type] = food;

            OnFoodUpdated();
        }

        private void OnFoodRemoved(FoodType foodType)
        {
            m_Foods[foodType] = null;
            OnFoodUpdated();
        }

        private void OnFoodUpdated()
        {
            m_Price.Set(m_Foods.Values.Where(f => f != null && !f.IsApplied).Sum(f => f.Price));

            View.UpdateSlots(m_Foods);
            View.UpdatePrice(m_Price.Amount, !m_Currencies.HasEnough(m_Price));
        }

        private void OnBuyButtonClicked()
        {
            try
            {
                m_Currencies.Withdraw(m_Price);
                m_FoodComponent.Apply(m_Foods);

                OnFoodUpdated();

                AudioManager.Instance.PlayItemBuy();
            }
            catch (GameplayException exception)
            {
                UiErrorFrame.Instance.ShowMessage(exception.Message);
            }
        }
    }
}