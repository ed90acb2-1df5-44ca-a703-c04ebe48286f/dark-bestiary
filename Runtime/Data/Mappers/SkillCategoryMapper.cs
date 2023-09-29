using DarkBestiary.Skills;

namespace DarkBestiary.Data.Mappers
{
    public class SkillCategoryMapper : Mapper<SkillCategoryData, SkillCategory>
    {
        public override SkillCategoryData ToData(SkillCategory entity)
        {
            throw new System.NotImplementedException();
        }

        public override SkillCategory ToEntity(SkillCategoryData data)
        {
            return new SkillCategory(data);
        }
    }
}