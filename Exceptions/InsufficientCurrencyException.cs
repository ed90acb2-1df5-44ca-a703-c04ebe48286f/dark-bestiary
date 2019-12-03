using DarkBestiary.Currencies;

namespace DarkBestiary.Exceptions
{
    public class InsufficientCurrencyException : GameplayException
    {
        public InsufficientCurrencyException(Currency currency)
            : base(I18N.Instance.Get("exception_not_enough_x").ToString(new object[] {currency.Name}))
        {
        }
    }
}