using System;
using DarkBestiary.Data;
using DarkBestiary.Validators;

namespace DarkBestiary.Achievements.Conditions
{
    public abstract class AchievementCondition
    {
        public event Action<AchievementCondition> Updated;

        protected readonly AchievementConditionData Data;

        private int m_Quantity;

        protected AchievementCondition(AchievementConditionData data)
        {
            Data = data;
        }

        protected void AddQuantity(int amount = 1)
        {
            m_Quantity += amount;
            Updated?.Invoke(this);
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();

        public void Reset()
        {
            m_Quantity = 0;
        }

        public bool Check()
        {
            return Comparator.Compare(m_Quantity, Data.RequiredQuantity, Data.Comparator);
        }
    }
}