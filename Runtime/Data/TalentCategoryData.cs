using System;

namespace DarkBestiary.Data
{
    [Serializable]
    public class TalentCategoryData : Identity<int>
    {
        public string NameKey;
        public int Index;
    }
}