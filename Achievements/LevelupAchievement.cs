using System.Collections.Generic;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Managers;

namespace DarkBestiary.Achievements
{
    public class LevelupAchievement : Achievement
    {
        private Character character;

        public LevelupAchievement(AchievementData data, List<AchievementCondition> conditions) : base(data, conditions)
        {
        }

        public override void Subscribe()
        {
            var experience = CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>();
            experience.Experience.LevelUp -= OnLevelUp;
            experience.Experience.LevelUp += OnLevelUp;
        }

        public override void Unsubscribe()
        {
            if (CharacterManager.Instance.Character == null)
            {
                return;
            }

            CharacterManager.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.LevelUp -= OnLevelUp;
        }

        private void OnLevelUp(Experience experience)
        {
            if (experience.Level < this.Data.Level)
            {
                return;
            }

            AddQuantity();
        }
    }
}