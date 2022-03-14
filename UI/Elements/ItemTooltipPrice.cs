using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipPrice : MonoBehaviour
    {
        [SerializeField] private CurrencyView currencyPrefab;

        private MonoBehaviourPool<CurrencyView> currencyPool;

        public void Initialize()
        {
            this.currencyPool = MonoBehaviourPool<CurrencyView>.Factory(this.currencyPrefab, transform);
        }

        public void Terminate()
        {
            this.currencyPool.Clear();
        }

        public void Refresh(List<Currency> price)
        {
            this.currencyPool.DespawnAll();

            foreach (var currency in price.Where(currency => currency.Amount > 0))
            {
                this.currencyPool.Spawn().Initialize(currency);
            }
        }
    }
}