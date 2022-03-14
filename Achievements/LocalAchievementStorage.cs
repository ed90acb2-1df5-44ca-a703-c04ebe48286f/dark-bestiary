using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary.Achievements
{
    public class LocalAchievementStorage : IAchievementStorage
    {
        private readonly IFileReader reader;
        private readonly StorageId storageId;

        public LocalAchievementStorage(IFileReader reader, StorageId storageId)
        {
            this.reader = reader;
            this.storageId = storageId;
        }

        public AchievementsSaveData Read()
        {
            return this.reader.Read<AchievementsSaveData>(GetDataPath()) ?? new AchievementsSaveData();
        }

        public void Write(AchievementsSaveData data)
        {
            this.reader.Write(data, GetDataPath());
        }

        private string GetDataPath()
        {
            return Environment.PersistentDataPath + $"/{this.storageId}/achievements.save";
        }
    }
}