using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkinFileRepository : FileRepository<int, SkinData, Skin>, ISkinRepository
    {
        public SkinFileRepository(IFileReader reader, SkinMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/skins.json";
        }
    }
}