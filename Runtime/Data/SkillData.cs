using System;
using System.Collections.Generic;
using DarkBestiary.Skills;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SkillData : Identity<int>
    {
        public bool IsEnabled;
        public string NameKey;
        public string LoreKey;
        public string DescriptionKey;
        public string Animation;
        public string Icon;
        public SkillType Type;
        public SkillFlags Flags;
        public SkillTargetType TargetType;
        public int RequiredItemCategoryId;
        public int RequiredLevel;
        public int Aoe;
        public Shape AoeShape;
        public int Cooldown;
        public int CategoryId;
        public int BehaviourId;
        public int EffectId;
        public int RarityId;
        public int RangeMin;
        public int RangeMax;
        public List<PriceData> Price = new();
        public List<int> Sets = new();
        public Shape RangeShape;
        public Dictionary<ResourceType, float> ResourcesCosts = new();
    }
}