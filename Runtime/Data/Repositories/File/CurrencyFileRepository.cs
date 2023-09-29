using System.Linq;
using DarkBestiary.Currencies;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class CurrencyFileRepository : FileRepository<int, CurrencyData, Currency>, ICurrencyRepository
    {
        public CurrencyFileRepository(IFileReader reader, CurrencyMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/currencies.json";
        }

        public Currency FindByType(CurrencyType type)
        {
            return LoadData().Where(data => data.Type == type).Select(Mapper.ToEntity).FirstOrDefault();
        }
    }
}