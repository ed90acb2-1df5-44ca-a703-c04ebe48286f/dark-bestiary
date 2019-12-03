using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkillSetFileRepository : FileRepository<int, SkillSetData, SkillSet>, ISkillSetRepository
    {
        public SkillSetFileRepository(IFileReader loader, SkillSetMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/skill_sets.json";
        }
    }
}