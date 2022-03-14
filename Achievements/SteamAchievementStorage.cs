#if !DISABLESTEAMWORKS

using System;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Readers;
using DarkBestiary.Data.Repositories;
using Steamworks;
using UnityEngine;

namespace DarkBestiary.Achievements
{
    public class SteamAchievementStorage : IAchievementStorage
    {
        private readonly IAchievementRepository achievementRepository;
        private readonly IFileReader reader;
        private readonly StorageId storageId;

        public SteamAchievementStorage(IAchievementRepository achievementRepository,
            IFileReader reader, StorageId storageId)
        {
            this.achievementRepository = achievementRepository;
            this.reader = reader;
            this.storageId = storageId;
        }

        public AchievementsSaveData Read()
        {
            var data = this.reader.Read<AchievementsSaveData>(GetDataPath()) ?? new AchievementsSaveData();

            if (SteamManager.Initialized)
            {
                SyncSteamAchievementStatuses(data);
            }

            return data;
        }

        public void Write(AchievementsSaveData data)
        {
            this.reader.Write(data, GetDataPath());
        }

        private string GetDataPath()
        {
            return Environment.PersistentDataPath + $"/{this.storageId}/achievements.save";
        }

        private void SyncSteamAchievementStatuses(AchievementsSaveData data)
        {
            foreach (var achievement in this.achievementRepository.FindAll())
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