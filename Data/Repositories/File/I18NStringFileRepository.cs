using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class I18NStringFileRepository : FileRepository<int, I18NStringData, I18NString>, II18NStringRepository
    {
        public I18NStringFileRepository(IFileReader reader, I18NStringMapper mapper) : base(reader, mapper)
        {
        }

        public I18NString FindByKey(string key)
        {
            var entry = LoadData().FirstOrDefault(d => d.Key == key);

            return entry == null ? default : this.Mapper.ToEntity(entry);
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/i18n.json";
        }
    }
}