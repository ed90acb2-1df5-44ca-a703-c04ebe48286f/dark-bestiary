#if !DISABLESTEAMWORKS

using DarkBestiary.Achievements;
using Steamworks;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class SteamAchievementUnlocker : MonoBehaviour
    {
        private void Start()
        {
            if (!SteamManager.Initialized)
            {
                return;
            }

            Achievement.AnyAchievementUnlocked += OnAnyAchievementUnlocked;
        }

        private static void OnAnyAchievementUnlocked(Achievement achievement)
        {
            SteamUserStats.SetAchievement(achievement.SteamApiKey);
            SteamUserStats.StoreStats();
        }
    }
}

#endif