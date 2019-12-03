using System;
using DarkBestiary.Validators;

namespace DarkBestiary.Data
{
    [Serializable]
    public class AchievementConditionData : Identity<int>
    {
        public string Type;
        public ComparatorMethod Comparator;
        public int RequiredQuantity;
        public int UnitId;
        public int BehaviourId;
    }
}