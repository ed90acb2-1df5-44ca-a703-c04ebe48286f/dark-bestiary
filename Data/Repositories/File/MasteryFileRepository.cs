using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Masteries;

namespace DarkBestiary.Data.Repositories.File
{
    public class MasteryFileRepository : FileRepository<int, MasteryData, Mastery>, IMasteryRepository
    {
        public MasteryFileRepository(IFileReader reader, MasteryMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/masteries.json";
        }
    }
}