using System.Collections.Generic;
using DarkBestiary.Achievements;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class AchievementsView : View, IAchievementsView
    {
        [SerializeField] private AchievementRow m_AchievementPrefab;
        [SerializeField] private Transform m_AchievementContainer;
        [SerializeField] private Interactable m_CloseButton;

        private readonly List<AchievementRow> m_AchievementRows = new();

        public void Construct(List<Achievement> achievements)
        {
            foreach (var achievement in achievements)
            {
                var achievementRow = Instantiate(m_AchievementPrefab, m_AchievementContainer);
                achievementRow.Initialize(achievement);

                m_AchievementRows.Add(achievementRow);
            }
        }

        protected override void OnInitialize()
        {
            m_CloseButton.PointerClick += OnCloseButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            m_CloseButton.PointerClick -= OnCloseButtonPointerClick;

            foreach (var achievementRow in m_AchievementRows)
            {
                achievementRow.Terminate();
            }
        }

        private void OnCloseButtonPointerClick()
        {
            Hide();
        }
    }
}