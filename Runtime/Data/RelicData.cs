using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class RelicData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public string LoreKey;
        public int BehaviourId;
        public int RarityId;
        public string Icon;
    }
}