using DarkBestiary.Exceptions;
using DarkBestiary.Messaging;

namespace DarkBestiary.Currencies
{
    public class Currency
    {
        public event Payload<Currency> Changed;

        public int Id { get; }
        public CurrencyType Type { get; }
        public int Amount { get; private set; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public string Icon { get; }

        public Currency(int id, CurrencyType type, int amount, string icon, I18NString name, I18NString description)
        {
            Id = id;
            Type = type;
            Amount = amount;
            Icon = icon;
            Name = name;
            Description = description;
        }

        public Currency Clone()
        {
            return new Currency(Id, Type, Amount, Icon, Name, Description);
        }

        public static Currency operator *(Currency currency, int value)
        {
            return new Currency(
                currency.Id,
                currency.Type,
                (currency.Amount * value),
                currency.Icon, currency.Name,
                currency.Description
            );
        }

        public static Currency operator *(Currency currency, float value)
        {
            return currency * (int) value;
        }

        public Currency Add(float amount)
        {
            Add((int) amount);
            return this;
        }

        public Currency Add(double amount)
        {
            Add((int) amount);
            return this;
        }

        public Currency Set(int amount)
        {
            Amount = amount;
            Changed?.Invoke(this);
            return this;
        }

        public Currency Add(int amount)
        {
            Amount += amount;
            Changed?.Invoke(this);
            return this;
        }

        public void Withdraw(int amount)
        {
            if (amount > Amount)
            {
                throw new InsufficientCurrencyException(this);
            }

            Amount -= amount;
            Changed?.Invoke(this);
        }
    }
}