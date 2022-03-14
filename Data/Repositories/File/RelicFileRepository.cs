using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class RelicFileRepository : FileRepository<int, RelicData, Relic>, IRelicRepository
    {
        public RelicFileRepository(IFileReader reader, RelicMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/relics.json";
        }
    }
}