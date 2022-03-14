using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Achievements
{
    public class AchievementManager : IInitializable
    {
        public List<Achievement> Achievements { get; private set; } = new List<Achievement>();

        private readonly IAchievementRepository achievementRepository;
        private readonly IAchievementStorage achievementStorage;

        private AchievementsSaveData data;

        public AchievementManager(IAchievementRepository achievementRepository,
            IAchievementStorage achievementStorage)
        {
            this.achievementRepository = achievementRepository;
            this.achievementStorage = achievementStorage;
        }

        public void Initialize()
        {
            try
            {
                this.data = this.achievementStorage.Read();
            }
            catch (Exception exception)
            {
                this.data = new AchievementsSaveData();
            }

            Achievements = this.achievementRepository.FindAll()
                .OrderBy(a => a.Index)
                .ToList();

            GameState.AnyGameStateEnter += OnAnyGameStateEnter;
            Application.quitting += OnApplicationQuitting;

            // Wait for "SteamAchievementUnlocker" ...
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                foreach (var achievement in Achievements)
                {
                    var status = this.data.AchievementStatuses.FirstOrDefault(data => data.AchievementId == achievement.Id);

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

        private void OnAnyGameStateEnter(GameState gameState)
        {
            foreach (var achievement in Achievements)
            {
                achievement.Unsubscribe();
            }

            if (gameState.IsHub || gameState.IsScenario)
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
            var achievementStatus = this.data.AchievementStatuses
                .FirstOrDefault(data => data.AchievementId == achievementId);

            if (achievementStatus != null)
            {
                return achievementStatus;
            }

            achievementStatus = new AchievementStatusData
            {
                AchievementId = achievementId
            };

            this.data.AchievementStatuses.Add(achievementStatus);

            return achievementStatus;
        }

        private void OnApplicationQuitting()
        {
            this.achievementStorage.Write(this.data);
        }
    }
}