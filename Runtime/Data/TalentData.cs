using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class TalentData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public int BehaviourId;
        public int CategoryId;
        public int Tier;
        public int Index;
        public string Icon;
    }
}