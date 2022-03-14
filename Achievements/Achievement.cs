using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Achievements.Conditions;
using DarkBestiary.Data;
using DarkBestiary.Messaging;

namespace DarkBestiary.Achievements
{
    public abstract class Achievement
    {
        public static event Payload<Achievement> AnyAchievementUnlocked;
        public static event Payload<Achievement> AnyAchievementUpdated;

        public event Payload<Achievement> Unlocked;
        public event Payload<Achievement> Updated;

        public int Id => this.Data.Id;
        public int Index => this.Data.Index;
        public string Icon => this.Data.Icon;
        public string SteamApiKey => this.Data.SteamApiKey;
        public int RequiredQuantity => this.Data.RequiredQuantity;
        public int Quantity { get; private set; }
        public List<AchievementCondition> Conditions { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public bool IsUnlocked { get; set; }

        protected readonly AchievementData Data;

        protected Achievement(AchievementData data, List<AchievementCondition> conditions)
        {
            this.Data = data;

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
            if (!this.Data.IsEnabled || IsUnlocked || Quantity < RequiredQuantity)
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