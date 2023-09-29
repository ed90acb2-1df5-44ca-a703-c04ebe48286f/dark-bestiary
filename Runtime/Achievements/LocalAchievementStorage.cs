using DarkBestiary.Data;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Achievements
{
    public class LocalAchievementStorage : IAchievementStorage
    {
        private readonly IFileReader m_Reader;
        private readonly StorageId m_StorageId;

        public LocalAchievementStorage(IFileReader reader, StorageId storageId)
        {
            m_Reader = reader;
            m_StorageId = storageId;
        }

        public AchievementsSaveData Read()
        {
            return m_Reader.Read<AchievementsSaveData>(GetDataPath()) ?? new AchievementsSaveData();
        }

        public void Write(AchievementsSaveData data)
        {
            m_Reader.Write(data, GetDataPath());
        }

        private string GetDataPath()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/achievements.save";
        }
    }
}