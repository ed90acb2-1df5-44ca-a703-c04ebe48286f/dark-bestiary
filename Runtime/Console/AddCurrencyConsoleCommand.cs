using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class AddCurrencyConsoleCommand : IConsoleCommand
    {
        private readonly ICurrencyRepository m_CurrencyRepository;

        public AddCurrencyConsoleCommand(ICurrencyRepository currencyRepository)
        {
            m_CurrencyRepository = currencyRepository;
        }

        public string GetSignature()
        {
            return "add_currency";
        }

        public string GetDescription()
        {
            return "Add currency. (Format: [currencyId] [amount])";
        }

        public string Execute(string input)
        {
            var options = input.Split();

            var currencyId = int.Parse(options[0]);
            var amount = 1;

            if (options.Length > 1)
            {
                amount = int.Parse(options[1]);
            }

            var currency = m_CurrencyRepository.FindOrFail(currencyId).Add(amount);

            Game.Instance.Character.Entity.GetComponent<CurrenciesComponent>().Give(currency);

            return $"{Game.Instance.Character.Name} received currency {currency.Name} x{currency.Amount}";
        }
    }
}