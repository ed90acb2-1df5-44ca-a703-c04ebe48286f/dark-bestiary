using DarkBestiary.Achievements;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class AchievementPopupManager : Singleton<AchievementPopupManager>
    {
        [SerializeField] private AchievementPopup achievementPopupPrefab;
        [SerializeField] private Transform achievementPopupContainer;

        private void Start()
        {
            Achievement.AnyAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            Instantiate(this.achievementPopupPrefab, this.achievementPopupContainer).Initialize(achievement);
        }
    }
}