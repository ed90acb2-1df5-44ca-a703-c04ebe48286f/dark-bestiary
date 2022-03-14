using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class EateryView : View, IEateryView
    {
        public event Payload<Food> FoodSelected;
        public event Payload<FoodType> FoodRemoved;
        public event Payload BuyButtonClicked;

        [SerializeField] private Interactable buyButton;
        [SerializeField] private Interactable closeButton;
        [SerializeField] private Interactable crossButton;
        [SerializeField] private TavernFood foodPrefab;
        [SerializeField] private Transform foodContainer;
        [SerializeField] private TavernFoodSlot foodSlotPrefab;
        [SerializeField] private Transform foodSlotContainer;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private CurrencyView currencyView;

        private readonly Dictionary<FoodType, TavernFoodSlot> foodSlotViews
            = new Dictionary<FoodType, TavernFoodSlot>();

        private readonly List<TavernFood> foodViews = new List<TavernFood>();

        private Color defaultPriceTextColor;
        private TavernFoodSlot activeFoodSlot;

        public void Construct(CurrenciesComponent currencies, IReadOnlyDictionary<FoodType, Food> foodSlots, List<Food> assortment)
        {
            this.defaultPriceTextColor = this.priceText.color;
            this.currencyView.Initialize(currencies.Get(CurrencyType.Gold));

            foreach (var foodSlot in foodSlots)
            {
                CreateFoodSlot(foodSlot.Key, foodSlot.Value);
            }

            CreateAssortment(assortment);

            this.buyButton.PointerClick += OnBuyButtonPointerClick;
            this.crossButton.PointerClick += OnCrossButtonPointerClick;
            this.closeButton.PointerClick += Hide;

            OnFoodSlotClicked(this.foodSlotViews.Values.First());
        }

        protected override void OnTerminate()
        {
            this.currencyView.Terminate();

            foreach (var foodView in this.foodViews)
            {
                foodView.Clicked -= OnFoodClicked;
                foodView.Terminate();
            }

            foreach (var foodSlotView in this.foodSlotViews.Values)
            {
                foodSlotView.Clicked -= OnFoodSlotClicked;
                foodSlotView.Terminate();
            }

            this.buyButton.PointerClick -= OnBuyButtonPointerClick;
            this.crossButton.PointerClick -= OnCrossButtonPointerClick;
            this.closeButton.PointerClick -= Hide;
        }

        public void UpdateSlots(IReadOnlyDictionary<FoodType, Food> foodSlots)
        {
            foreach (var foodSlot in foodSlots)
            {
                this.foodSlotViews[foodSlot.Key].Change(foodSlot.Value);
            }

            RefreshActiveFood();
        }

        public void UpdatePrice(int price, bool isTooExpensive)
        {
            this.priceText.color = isTooExpensive ? Color.red : this.defaultPriceTextColor;
            this.priceText.text = price.ToString();
        }

        private void CreateAssortment(List<Food> assortment)
        {
            foreach (var food in assortment)
            {
                CreateFood(food);
            }
        }

        private void CreateFoodSlot(FoodType type, Food food)
        {
            var foodSlotView = Instantiate(this.foodSlotPrefab, this.foodSlotContainer);
            foodSlotView.Clicked += OnFoodSlotClicked;
            foodSlotView.Initialize(type, food);
            this.foodSlotViews.Add(type, foodSlotView);
        }

        private void CreateFood(Food food)
        {
            var foodView = Instantiate(this.foodPrefab, this.foodContainer);
            foodView.Clicked += OnFoodClicked;
            foodView.Initialize(food);
            this.foodViews.Add(foodView);
        }

        private void RefreshAssortment()
        {
            this.titleText.text = EnumTranslator.Translate(this.activeFoodSlot.Type);

            foreach (var foodView in this.foodViews)
            {
                foodView.gameObject.SetActive(foodView.Food.Type == this.activeFoodSlot.Type);
            }

            this.crossButton.transform.SetAsLastSibling();
        }

        private void RefreshActiveFood()
        {
            var activeFood = this.activeFoodSlot.Food;

            foreach (var foodView in this.foodViews)
            {
                if (activeFood?.Id == foodView.Food.Id)
                {
                    foodView.Select();
                }
                else
                {
                    foodView.Deselect();
                }
            }
        }

        private void OnCrossButtonPointerClick()
        {
            FoodRemoved?.Invoke(this.activeFoodSlot.Type);
        }

        private void OnBuyButtonPointerClick()
        {
            BuyButtonClicked?.Invoke();
        }

        private void OnFoodClicked(TavernFood food)
        {
            FoodSelected?.Invoke(food.Food);

            RefreshActiveFood();
        }

        private void OnFoodSlotClicked(TavernFoodSlot foodSlot)
        {
            if (this.activeFoodSlot != null)
            {
                this.activeFoodSlot.Deselect();
            }

            this.activeFoodSlot = foodSlot;
            this.activeFoodSlot.Select();

            RefreshAssortment();
            RefreshActiveFood();
        }
    }
}