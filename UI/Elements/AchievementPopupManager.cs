using DarkBestiary.Achievements;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class AchievementPopupManager : MonoBehaviour
    {
        [SerializeField] private AchievementPopup achievementPopupPrefab;
        [SerializeField] private Transform achievementPopupContainer;

        private void Start()
        {
            Achievement.AnyAchievementUnlocked += OnAchievementUnlocked;
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            if (Game.Instance == null)
            {
                return;
            }

            Instantiate(this.achievementPopupPrefab, this.achievementPopupContainer).Initialize(achievement);
        }
    }
}