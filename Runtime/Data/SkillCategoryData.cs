using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SkillCategoryData : Identity<int>
    {
        public string NameKey;
        public int Index;
    }
}