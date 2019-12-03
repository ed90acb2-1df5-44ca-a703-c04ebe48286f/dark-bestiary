using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Exceptions;
using DarkBestiary.Messaging;

namespace DarkBestiary.Currencies
{
    public class CurrenciesComponent : Component
    {
        public event Payload<Currency> AnyCurrencyChanged;

        public List<Currency> Currencies { get; private set; }

        public CurrenciesComponent Construct(List<Currency> currencies)
        {
            Currencies = currencies;
            return this;
        }

        protected override void OnInitialize()
        {
            foreach (var currency in Currencies)
            {
                currency.Changed += OnCurrencyChanged;
            }
        }

        protected override void OnTerminate()
        {
            foreach (var currency in Currencies)
            {
                currency.Changed -= OnCurrencyChanged;
            }
        }

        public bool HasEnough(Currency currency)
        {
            return Get(currency.Type)?.Amount >= currency.Amount;
        }

        public bool HasEnough(List<Currency> currencies)
        {
            return currencies.All(HasEnough);
        }

        public Currency Get(CurrencyType type)
        {
            return Currencies.FirstOrDefault(currency => currency.Type == type);
        }

        public void Withdraw(Currency currency)
        {
            if (!HasEnough(currency))
            {
                throw new InsufficientCurrencyException(currency);
            }

            Currencies.First(element => element.Type == currency.Type).Withdraw(currency.Amount);
        }

        public void Withdraw(List<Currency> currencies)
        {
            var insufficientCurrency = GetInsufficientCurrency(currencies);

            if (insufficientCurrency != null)
            {
                throw new InsufficientCurrencyException(insufficientCurrency);
            }

            foreach (var currency in currencies)
            {
                Withdraw(currency);
            }
        }

        public void Give(CurrencyType type, int amount)
        {
            Currencies.First(element => element.Type == type).Add(amount);
        }

        public void Give(Currency currency)
        {
            Currencies.First(element => element.Type == currency.Type).Add(currency.Amount);
        }

        public void Give(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                Give(currency);
            }
        }

        private Currency GetInsufficientCurrency(List<Currency> currencies)
        {
            foreach (var currency in currencies)
            {
                if (HasEnough(currency))
                {
                    continue;
                }

                return Get(currency.Type);
            }

            return null;
        }

        private void OnCurrencyChanged(Currency currency)
        {
            AnyCurrencyChanged?.Invoke(currency);
        }
    }
}