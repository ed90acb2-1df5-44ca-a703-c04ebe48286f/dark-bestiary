using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Skills;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkillCategoryFileRepository
        : FileRepository<int, SkillCategoryData, SkillCategory>, ISkillCategoryRepository
    {
        public SkillCategoryFileRepository(IFileReader loader, SkillCategoryMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/skill_categories.json";
        }
    }
}