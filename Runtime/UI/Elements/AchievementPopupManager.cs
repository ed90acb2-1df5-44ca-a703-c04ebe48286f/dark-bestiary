using DarkBestiary.Achievements;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class AchievementPopupManager : MonoBehaviour
    {
        [SerializeField] private AchievementPopup m_AchievementPopupPrefab;
        [SerializeField] private Transform m_AchievementPopupContainer;

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

            Instantiate(m_AchievementPopupPrefab, m_AchievementPopupContainer).Initialize(achievement);
        }
    }
}