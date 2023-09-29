using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SkillSetData : Identity<int>
    {
        public string NameKey;
        public string Icon;
        public List<int> Skills;
        public List<SkillSetBehaviourData> Behaviours = new();
    }

    [Serializable]
    public class SkillSetBehaviourData
    {
        public int SkillCount;
        public List<int> Behaviours;
    }
}