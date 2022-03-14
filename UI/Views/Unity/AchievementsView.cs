using System.Collections.Generic;
using DarkBestiary.Achievements;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class AchievementsView : View, IAchievementsView
    {
        [SerializeField] private AchievementRow achievementPrefab;
        [SerializeField] private Transform achievementContainer;
        [SerializeField] private Interactable closeButton;

        private readonly List<AchievementRow> achievementRows = new List<AchievementRow>();

        public void Construct(List<Achievement> achievements)
        {
            foreach (var achievement in achievements)
            {
                var achievementRow = Instantiate(this.achievementPrefab, this.achievementContainer);
                achievementRow.Initialize(achievement);

                this.achievementRows.Add(achievementRow);
            }
        }

        protected override void OnInitialize()
        {
            this.closeButton.PointerClick += OnCloseButtonPointerClick;
        }

        protected override void OnTerminate()
        {
            this.closeButton.PointerClick -= OnCloseButtonPointerClick;

            foreach (var achievementRow in this.achievementRows)
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