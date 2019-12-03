using DarkBestiary.Data;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;

namespace DarkBestiary.Achievements.Conditions
{
    public abstract class AchievementCondition
    {
        public event Payload<AchievementCondition> Updated;

        protected readonly AchievementConditionData Data;

        private int quantity;

        protected AchievementCondition(AchievementConditionData data)
        {
            this.Data = data;
        }

        protected void AddQuantity(int amount = 1)
        {
            this.quantity += amount;
            Updated?.Invoke(this);
        }

        public abstract void Subscribe();

        public abstract void Unsubscribe();

        public void Reset()
        {
            this.quantity = 0;
        }

        public bool Check()
        {
            return Comparator.Compare(this.quantity, this.Data.RequiredQuantity, this.Data.Comparator);
        }
    }
}