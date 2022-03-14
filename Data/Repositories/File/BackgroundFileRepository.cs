using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class BackgroundFileRepository : FileRepository<int, BackgroundData, Background>, IBackgroundRepository
    {
        public BackgroundFileRepository(IFileReader reader, BackgroundMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/backgrounds.json";
        }
    }
}