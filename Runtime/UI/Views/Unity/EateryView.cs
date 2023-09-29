using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.UI.Elements;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class EateryView : View, IEateryView
    {
        public event Action<Food> FoodSelected;
        public event Action<FoodType> FoodRemoved;
        public event Action BuyButtonClicked;

        [SerializeField] private Interactable m_BuyButton;
        [SerializeField] private Interactable m_CloseButton;
        [SerializeField] private Interactable m_CrossButton;
        [SerializeField] private TavernFood m_FoodPrefab;
        [SerializeField] private Transform m_FoodContainer;
        [SerializeField] private TavernFoodSlot m_FoodSlotPrefab;
        [SerializeField] private Transform m_FoodSlotContainer;
        [SerializeField] private TextMeshProUGUI m_TitleText;
        [SerializeField] private TextMeshProUGUI m_PriceText;
        [SerializeField] private CurrencyView m_CurrencyView;

        private readonly Dictionary<FoodType, TavernFoodSlot> m_FoodSlotViews = new();

        private readonly List<TavernFood> m_FoodViews = new();

        private Color m_DefaultPriceTextColor;
        private TavernFoodSlot m_ActiveFoodSlot;

        public void Construct(CurrenciesComponent currencies, IReadOnlyDictionary<FoodType, Food> foodSlots, List<Food> assortment)
        {
            m_DefaultPriceTextColor = m_PriceText.color;
            m_CurrencyView.Initialize(currencies.Get(CurrencyType.Gold));

            foreach (var foodSlot in foodSlots)
            {
                CreateFoodSlot(foodSlot.Key, foodSlot.Value);
            }

            CreateAssortment(assortment);

            m_BuyButton.PointerClick += OnBuyButtonPointerClick;
            m_CrossButton.PointerClick += OnCrossButtonPointerClick;
            m_CloseButton.PointerClick += Hide;

            OnFoodSlotClicked(m_FoodSlotViews.Values.First());
        }

        protected override void OnTerminate()
        {
            m_CurrencyView.Terminate();

            foreach (var foodView in m_FoodViews)
            {
                foodView.Clicked -= OnFoodClicked;
                foodView.Terminate();
            }

            foreach (var foodSlotView in m_FoodSlotViews.Values)
            {
                foodSlotView.Clicked -= OnFoodSlotClicked;
                foodSlotView.Terminate();
            }

            m_BuyButton.PointerClick -= OnBuyButtonPointerClick;
            m_CrossButton.PointerClick -= OnCrossButtonPointerClick;
            m_CloseButton.PointerClick -= Hide;
        }

        public void UpdateSlots(IReadOnlyDictionary<FoodType, Food> foodSlots)
        {
            foreach (var foodSlot in foodSlots)
            {
                m_FoodSlotViews[foodSlot.Key].Change(foodSlot.Value);
            }

            RefreshActiveFood();
        }

        public void UpdatePrice(int price, bool isTooExpensive)
        {
            m_PriceText.color = isTooExpensive ? Color.red : m_DefaultPriceTextColor;
            m_PriceText.text = price.ToString();
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
            var foodSlotView = Instantiate(m_FoodSlotPrefab, m_FoodSlotContainer);
            foodSlotView.Clicked += OnFoodSlotClicked;
            foodSlotView.Initialize(type, food);
            m_FoodSlotViews.Add(type, foodSlotView);
        }

        private void CreateFood(Food food)
        {
            var foodView = Instantiate(m_FoodPrefab, m_FoodContainer);
            foodView.Clicked += OnFoodClicked;
            foodView.Initialize(food);
            m_FoodViews.Add(foodView);
        }

        private void RefreshAssortment()
        {
            m_TitleText.text = EnumTranslator.Translate(m_ActiveFoodSlot.Type);

            foreach (var foodView in m_FoodViews)
            {
                foodView.gameObject.SetActive(foodView.Food.Type == m_ActiveFoodSlot.Type);
            }

            m_CrossButton.transform.SetAsLastSibling();
        }

        private void RefreshActiveFood()
        {
            var activeFood = m_ActiveFoodSlot.Food;

            foreach (var foodView in m_FoodViews)
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
            FoodRemoved?.Invoke(m_ActiveFoodSlot.Type);
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
            if (m_ActiveFoodSlot != null)
            {
                m_ActiveFoodSlot.Deselect();
            }

            m_ActiveFoodSlot = foodSlot;
            m_ActiveFoodSlot.Select();

            RefreshAssortment();
            RefreshActiveFood();
        }
    }
}