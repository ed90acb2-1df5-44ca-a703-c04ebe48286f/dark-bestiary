using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class CurrencyFileRepository : FileRepository<int, CurrencyData, Currency>, ICurrencyRepository
    {
        public CurrencyFileRepository(IFileReader loader, CurrencyMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/currencies.json";
        }

        public Currency FindByType(CurrencyType type)
        {
            return LoadData().Where(data => data.Type == type).Select(this.Mapper.ToEntity).FirstOrDefault();
        }
    }
}