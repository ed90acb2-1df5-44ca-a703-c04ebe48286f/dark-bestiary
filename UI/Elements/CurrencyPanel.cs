using System.Collections.Generic;
using DarkBestiary.Currencies;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CurrencyPanel : MonoBehaviour
    {
        [SerializeField] private CurrencyView prefab;
        [SerializeField] private Transform container;

        public void Initialize(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                Instantiate(this.prefab, this.container).Initialize(currency);
            }
        }

        public void Terminate()
        {
            foreach (var currency in this.container.GetComponentsInChildren<CurrencyView>())
            {
                currency.Terminate();
            }
        }
    }
}