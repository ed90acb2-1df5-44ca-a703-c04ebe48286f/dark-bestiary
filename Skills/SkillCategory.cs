using DarkBestiary.Data;

namespace DarkBestiary.Skills
{
    public class SkillCategory
    {
        public static readonly SkillCategory All = new SkillCategory(-1, -1, I18N.Instance.Get("ui_all"));

        public int Id { get; }
        public int Index { get; }
        public I18NString Name { get; }

        public SkillCategory(int id, int index, I18NString name)
        {
            Id = id;
            Index = index;
            Name = name;
        }

        public SkillCategory(SkillCategoryData data)
        {
            Id = data.Id;
            Index = data.Index;
            Name = I18N.Instance.Get(data.NameKey);
        }
    }
}