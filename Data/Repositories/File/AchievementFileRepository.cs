using DarkBestiary.Achievements;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class AchievementFileRepository : FileRepository<int, AchievementData, Achievement>, IAchievementRepository
    {
        public AchievementFileRepository(IFileReader loader, AchievementMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/achievements.json";
        }
    }
}