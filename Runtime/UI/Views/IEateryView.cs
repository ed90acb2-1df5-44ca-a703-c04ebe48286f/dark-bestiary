using System;
using System.Collections.Generic;
using DarkBestiary.Currencies;

namespace DarkBestiary.UI.Views
{
    public interface IEateryView : IView, IHideOnEscape
    {
        event Action<Food> FoodSelected;
        event Action<FoodType> FoodRemoved;
        event Action BuyButtonClicked;

        void Construct(CurrenciesComponent currencies, IReadOnlyDictionary<FoodType, Food> foodSlots, List<Food> assortment);
        void UpdateSlots(IReadOnlyDictionary<FoodType, Food> foodSlots);
        void UpdatePrice(int price, bool isTooExpensive);
    }
}