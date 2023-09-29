using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Skills;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkillCategoryFileRepository
        : FileRepository<int, SkillCategoryData, SkillCategory>, ISkillCategoryRepository
    {
        public SkillCategoryFileRepository(IFileReader reader, SkillCategoryMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/skill_categories.json";
        }
    }
}