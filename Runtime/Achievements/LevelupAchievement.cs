using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Components;
using DarkBestiary.Data;

namespace DarkBestiary.Achievements
{
    public class LevelupAchievement : Achievement
    {
        private Character m_Character;

        public LevelupAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            var experience = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>();
            experience.Experience.LevelUp -= OnLevelUp;
            experience.Experience.LevelUp += OnLevelUp;
        }

        public override void Unsubscribe()
        {
            if (Game.Instance.Character == null)
            {
                return;
            }

            Game.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.LevelUp -= OnLevelUp;
        }

        private void OnLevelUp(Experience experience)
        {
            if (experience.Level < Data.Level)
            {
                return;
            }

            AddQuantity();
        }
    }
}