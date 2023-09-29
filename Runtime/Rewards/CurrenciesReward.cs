using System.Collections.Generic;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class CurrenciesReward : Reward
    {
        public List<Currency> Currencies { get; private set; }

        private readonly CurrenciesRewardData m_Data;
        private readonly ICurrencyRepository m_CurrencyRepository;

        public CurrenciesReward(CurrenciesRewardData data, ICurrencyRepository currencyRepository)
        {
            m_Data = data;
            m_CurrencyRepository = currencyRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            Currencies = new List<Currency>();

            foreach (var currencyData in m_Data.Currencies)
            {
                var currency = m_CurrencyRepository.Find(currencyData.CurrencyId);
                currency.Add(currencyData.Amount);
                Currencies.Add(currency);
            }
        }

        protected override void OnClaim(GameObject entity)
        {
            entity.GetComponent<CurrenciesComponent>().Give(Currencies);
        }
    }
}