using DarkBestiary.Currencies;

namespace DarkBestiary.Data.Repositories
{
    public interface ICurrencyRepository : IRepository<int, Currency>
    {
        Currency FindByType(CurrencyType type);
    }
}