using System;
using System.Collections.Generic;

namespace DarkBestiary.Data
{
    [Serializable]
    public class BehaviourTreeData : Identity<int>
    {
        public string Type;
        public BehaviourTreePropertiesData Properties;
        public List<BehaviourTreeData> Children = new();
    }

    [Serializable]
    public class BehaviourTreePropertiesData
    {
        public int SkillId;
        public int UnitId;
        public float WaitDuration;
        public float Chance;
        public int Count;
        public ResourceType ResourceType;
        public float ResourceAmount;
        public int Range;
        public float HealthFraction;
    }
}