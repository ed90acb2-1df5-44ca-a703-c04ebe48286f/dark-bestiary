using DarkBestiary.Currencies;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class AddCurrencyConsoleCommand : IConsoleCommand
    {
        private readonly CharacterManager characterManager;
        private readonly ICurrencyRepository currencyRepository;

        public AddCurrencyConsoleCommand(CharacterManager characterManager, ICurrencyRepository currencyRepository)
        {
            this.characterManager = characterManager;
            this.currencyRepository = currencyRepository;
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

            var currency = this.currencyRepository.FindOrFail(currencyId).Add(amount);

            this.characterManager.Character.Entity.GetComponent<CurrenciesComponent>().Give(currency);

            return $"{this.characterManager.Character.Name} received currency {currency.Name} x{currency.Amount}";
        }
    }
}