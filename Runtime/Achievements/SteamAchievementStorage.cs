#if !DISABLESTEAMWORKS

using System;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Repositories;
using Steamworks;

namespace DarkBestiary.Achievements
{
    public class SteamAchievementStorage : IAchievementStorage
    {
        private readonly IAchievementRepository m_AchievementRepository;
        private readonly IFileReader m_Reader;
        private readonly StorageId m_StorageId;

        public SteamAchievementStorage(IAchievementRepository achievementRepository,
            IFileReader reader, StorageId storageId)
        {
            m_AchievementRepository = achievementRepository;
            m_Reader = reader;
            m_StorageId = storageId;
        }

        public AchievementsSaveData Read()
        {
            var data = m_Reader.Read<AchievementsSaveData>(GetDataPath()) ?? new AchievementsSaveData();

            if (SteamManager.Initialized)
            {
                SyncSteamAchievementStatuses(data);
            }

            return data;
        }

        public void Write(AchievementsSaveData data)
        {
            m_Reader.Write(data, GetDataPath());
        }

        private string GetDataPath()
        {
            return Environment.s_PersistentDataPath + $"/{m_StorageId}/achievements.save";
        }

        private void SyncSteamAchievementStatuses(AchievementsSaveData data)
        {
            foreach (var achievement in m_AchievementRepository.FindAll())
            {
                var status = data.AchievementStatuses.FirstOrDefault(s => s.AchievementId == achievement.Id);

                if (status == null)
                {
                    status = new AchievementStatusData {AchievementId = achievement.Id};
                    data.AchievementStatuses.Add(status);
                }

                SteamUserStats.GetAchievementAndUnlockTime(
                    achievement.SteamApiKey, out var isUnlocked, out var unlockedAt);

                if (isUnlocked)
                {
                    status.Quantity = achievement.RequiredQuantity;
                    status.IsUnlocked = true;
                    status.UnlockedAt = unlockedAt;
                }
                else
                {
                    status.Quantity = Math.Min(status.Quantity, achievement.RequiredQuantity);
                    status.IsUnlocked = false;
                    status.UnlockedAt = 0;
                }
            }
        }
    }
}

#endif