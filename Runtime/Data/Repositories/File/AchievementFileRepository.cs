using DarkBestiary.Achievements;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class AchievementFileRepository : FileRepository<int, AchievementData, Achievement>, IAchievementRepository
    {
        public AchievementFileRepository(IFileReader reader, AchievementMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/achievements.json";
        }
    }
}