using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class FoodData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public string Icon;
        public FoodType Type;
        public int BehaviourId;
        public int Price;
    }
}