using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Data;

namespace DarkBestiary.Achievements
{
    public abstract class Achievement
    {
        public static event Action<Achievement> AnyAchievementUnlocked;
        public static event Action<Achievement> AnyAchievementUpdated;

        public event Action<Achievement> Unlocked;
        public event Action<Achievement> Updated;

        public int Id => Data.Id;
        public int Index => Data.Index;
        public string Icon => Data.Icon;
        public string SteamApiKey => Data.SteamApiKey;
        public int RequiredQuantity => Data.RequiredQuantity;
        public int Quantity { get; private set; }
        public List<AchievementCondition> Conditions { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public bool IsUnlocked { get; set; }

        protected readonly AchievementData Data;

        protected Achievement(AchievementData data, List<AchievementCondition> conditions)
        {
            Data = data;

            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Conditions = conditions;
        }

        public virtual void Subscribe()
        {
            foreach (var condition in Conditions)
            {
                condition.Subscribe();
            }
        }

        public virtual void Unsubscribe()
        {
            foreach (var condition in Conditions)
            {
                condition.Unsubscribe();
            }
        }

        protected void AddQuantity(int amount = 1)
        {
            if (Conditions.Any(c => !c.Check()))
            {
                return;
            }

            ChangeQuantity(Quantity + amount);
        }

        public void ChangeQuantity(int amount)
        {
            Quantity = Math.Min(amount, RequiredQuantity);
            AnyAchievementUpdated?.Invoke(this);
            Updated?.Invoke(this);

            Evaluate();
        }

        public void Evaluate()
        {
            if (!Data.IsEnabled || IsUnlocked || Quantity < RequiredQuantity)
            {
                return;
            }

            IsUnlocked = true;
            AnyAchievementUnlocked?.Invoke(this);
            Unlocked?.Invoke(this);

            Unsubscribe();
        }
    }
}