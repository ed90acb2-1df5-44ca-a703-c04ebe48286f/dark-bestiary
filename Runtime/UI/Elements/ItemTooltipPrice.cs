using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Currencies;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ItemTooltipPrice : MonoBehaviour
    {
        [SerializeField] private CurrencyView m_CurrencyPrefab;

        private MonoBehaviourPool<CurrencyView> m_CurrencyPool;

        public void Initialize()
        {
            m_CurrencyPool = MonoBehaviourPool<CurrencyView>.Factory(m_CurrencyPrefab, transform);
        }

        public void Terminate()
        {
            m_CurrencyPool.Clear();
        }

        public void Refresh(List<Currency> price)
        {
            m_CurrencyPool.DespawnAll();

            foreach (var currency in price.Where(currency => currency.Amount > 0))
            {
                m_CurrencyPool.Spawn().Initialize(currency);
            }
        }
    }
}