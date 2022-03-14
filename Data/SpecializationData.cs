using System.Collections.Generic;

namespace DarkBestiary.Data
{
    public class SpecializationData : Identity<int>
    {
        public string NameKey;
        public string DescriptionKey;
        public string Icon;
        public int SkillSetId;
        public List<SpecializationSkillData> Skills = new List<SpecializationSkillData>();
    }

    public class SpecializationSkillData
    {
        public int Id;
        public string Label;
        public int SkillId;
        public bool IsUnlocked;
        public int YOffset;
        public List<SpecializationSkillData> Skills = new List<SpecializationSkillData>();
    }
}