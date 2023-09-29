using System.Collections.Generic;
using DarkBestiary.Currencies;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class CurrencyPanel : MonoBehaviour
    {
        [SerializeField] private CurrencyView m_Prefab;
        [SerializeField] private Transform m_Container;

        public void Initialize(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                Instantiate(m_Prefab, m_Container).Initialize(currency);
            }
        }

        public void Terminate()
        {
            foreach (var currency in m_Container.GetComponentsInChildren<CurrencyView>())
            {
                currency.Terminate();
            }
        }
    }
}