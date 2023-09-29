using System;

namespace DarkBestiary.Components
{
    public class ExperienceComponent : Component
    {
        public const int c_MaxLevel = 100;

        public Experience Experience { get; private set; }

        public ExperienceComponent Construct(int level, int experience)
        {
            Experience = new Experience(level, c_MaxLevel, experience, RequiredExperienceAtLevel);
            return this;
        }

        protected override void OnInitialize()
        {
            Experience.LevelUp += OnLevelUp;
            OnLevelUp(Experience);
        }

        protected override void OnTerminate()
        {
            Experience.LevelUp -= OnLevelUp;
        }

        public void GiveExperience(int experience)
        {
            Experience.CreateSnapshot(experience);
            Experience.Add(experience);
        }

        private void OnLevelUp(Experience experience)
        {
            var unit = GetComponent<UnitComponent>();

            if (unit != null)
            {
                unit.Level = experience.Level;
            }
        }

        public static int RequiredExperienceAtLevel(int level)
        {
            if (level < 2)
            {
                return 0;
            }

            var extra = 6;

            if (level > 30)
            {
                extra = (int) Math.Pow(level - 30, 3.55f);
            }

            return (int) Math.Round(12 * Math.Pow(level, 2.145f) / 2) + extra;
        }
    }
}