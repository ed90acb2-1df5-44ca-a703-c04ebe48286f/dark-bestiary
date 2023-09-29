using System;
using DarkBestiary.Currencies;

namespace DarkBestiary.Data.Mappers
{
    public class CurrencyMapper : Mapper<CurrencyData, Currency>
    {
        public override CurrencyData ToData(Currency target)
        {
            throw new NotImplementedException();
        }

        public override Currency ToEntity(CurrencyData data)
        {
            return new Currency(data.Id, data.Type, 0, data.Icon, I18N.Instance.Get(data.NameKey), I18N.Instance.Get(data.DescriptionKey));
        }
    }
}