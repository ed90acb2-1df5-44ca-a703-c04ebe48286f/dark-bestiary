using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class MissileFileRepository : FileRepository<int, MissileData, Missile>, IMissileRepository
    {
        public MissileFileRepository(IFileReader reader, MissileMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/missiles.json";
        }
    }
}