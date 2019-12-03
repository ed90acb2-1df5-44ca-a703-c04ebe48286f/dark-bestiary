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

        private readonly CurrenciesRewardData data;
        private readonly ICurrencyRepository currencyRepository;

        public CurrenciesReward(CurrenciesRewardData data, ICurrencyRepository currencyRepository)
        {
            this.data = data;
            this.currencyRepository = currencyRepository;
        }

        protected override void OnPrepare(GameObject entity)
        {
            Currencies = new List<Currency>();

            foreach (var currencyData in this.data.Currencies)
            {
                var currency = this.currencyRepository.Find(currencyData.CurrencyId);
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