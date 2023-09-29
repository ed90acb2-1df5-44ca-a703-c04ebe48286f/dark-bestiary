using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Achievements
{
    public class AchievementManager : IInitializable
    {
        public List<Achievement> Achievements { get; private set; } = new();

        private readonly IAchievementRepository m_AchievementRepository;
        private readonly IAchievementStorage m_AchievementStorage;

        private AchievementsSaveData m_Data;

        public AchievementManager(IAchievementRepository achievementRepository,
            IAchievementStorage achievementStorage)
        {
            m_AchievementRepository = achievementRepository;
            m_AchievementStorage = achievementStorage;
        }

        public void Initialize()
        {
            try
            {
                m_Data = m_AchievementStorage.Read();
            }
            catch (Exception exception)
            {
                m_Data = new AchievementsSaveData();
            }

            Achievements = m_AchievementRepository.FindAll()
                .OrderBy(a => a.Index)
                .ToList();

            Game.Instance.SceneSwitched += OnSceneSwitched;
            Application.quitting += OnApplicationQuitting;

            // Wait for "SteamAchievementUnlocker" ...
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                foreach (var achievement in Achievements)
                {
                    var status = m_Data.AchievementStatuses.FirstOrDefault(data => data.AchievementId == achievement.Id);

                    if (status != null)
                    {
                        achievement.ChangeQuantity(status.Quantity);
                        achievement.Evaluate();
                    }

                    achievement.Unlocked += OnAchievementUnlocked;
                    achievement.Updated += OnAchievementUpdated;
                }
            });
        }

        private void OnSceneSwitched()
        {
            foreach (var achievement in Achievements)
            {
                achievement.Unsubscribe();
            }

            if (Game.Instance.IsTown || Game.Instance.IsScenario)
            {
                foreach (var achievement in Achievements)
                {
                    achievement.Conditions.ForEach(c => c.Reset());
                    achievement.Subscribe();
                }
            }
        }

        private void OnAchievementUpdated(Achievement achievement)
        {
            var achievementStatus = GetOrCreateAchievementStatus(achievement.Id);
            achievementStatus.Quantity = achievement.Quantity;
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            var achievementStatus = GetOrCreateAchievementStatus(achievement.Id);
            achievementStatus.UnlockedAt = DateTime.Now.ToUnixTimestamp();
            achievementStatus.IsUnlocked = true;
        }

        private AchievementStatusData GetOrCreateAchievementStatus(int achievementId)
        {
            var achievementStatus = m_Data.AchievementStatuses
                .FirstOrDefault(data => data.AchievementId == achievementId);

            if (achievementStatus != null)
            {
                return achievementStatus;
            }

            achievementStatus = new AchievementStatusData
            {
                AchievementId = achievementId
            };

            m_Data.AchievementStatuses.Add(achievementStatus);

            return achievementStatus;
        }

        private void OnApplicationQuitting()
        {
            m_AchievementStorage.Write(m_Data);
        }
    }
}