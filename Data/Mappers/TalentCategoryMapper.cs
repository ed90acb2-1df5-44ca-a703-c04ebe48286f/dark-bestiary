using DarkBestiary.Skills;
using DarkBestiary.Talents;

namespace DarkBestiary.Data.Mappers
{
    public class TalentCategoryMapper : Mapper<TalentCategoryData, TalentCategory>
    {
        public override TalentCategoryData ToData(TalentCategory entity)
        {
            throw new System.NotImplementedException();
        }

        public override TalentCategory ToEntity(TalentCategoryData data)
        {
            return new TalentCategory(data);
        }
    }
}