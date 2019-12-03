using System;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary
{
    public class Experience
    {
        public event Payload<Experience> LevelUp;
        public event Payload<Experience> Changed;

        public Experience Snapshot { get; private set; }

        public int Level { get; private set; }
        public int MaxLevel { get; }
        public int Current { get; private set; }
        public int Pending { get; private set; }

        private readonly Func<int, int> formula;

        public Experience(int level, int maxLevel, int experience, Func<int, int> formula)
        {
            this.formula = formula;

            MaxLevel = maxLevel;
            Level = Math.Max(1, Math.Min(level, maxLevel));
            Current = Math.Max(experience, RequiredAtLevel(level));
        }

        public int GetObtained()
        {
            return Math.Max(0, Current - RequiredAtLevel(Level));
        }

        public int GetRequired()
        {
            return RequiredNextLevel() - RequiredAtLevel(Level);
        }

        public float GetObtainedFraction()
        {
            return Mathf.Min(1.0f, (float) GetObtained() / GetRequired());
        }

        public void CreateSnapshot(int pending)
        {
            Snapshot = new Experience(Level, MaxLevel, Current, this.formula)
            {
                Pending = pending
            };
        }

        public void ResetToLevel(int level)
        {
            Level = level;
            Current = RequiredAtLevel(level);
        }

        public void Add(int amount)
        {
            if (Level >= MaxLevel)
            {
                return;
            }

            Current += amount;
            Changed?.Invoke(this);

            while (Current >= RequiredNextLevel())
            {
                Level = Math.Min(MaxLevel, Level + 1);

                LevelUp?.Invoke(this);
                Changed?.Invoke(this);

                if (Level < MaxLevel)
                {
                    continue;
                }

                Level = MaxLevel;
                Current = RequiredNextLevel();
                break;
            }
        }

        public int RequiredNextLevel()
        {
            return RequiredAtLevel(Level + 1);
        }

        public int RequiredAtLevel(int level)
        {
            return this.formula.Invoke(level);
        }
    }
}