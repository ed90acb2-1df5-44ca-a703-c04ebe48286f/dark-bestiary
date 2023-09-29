using DarkBestiary.Data;

namespace DarkBestiary.Talents
{
    public class TalentCategory
    {
        public int Id { get; }
        public int Index { get; }
        public I18NString Name { get; }

        public TalentCategory(TalentCategoryData data)
        {
            Id = data.Id;
            Index = data.Index;
            Name = I18N.Instance.Get(data.NameKey);
        }
    }
}