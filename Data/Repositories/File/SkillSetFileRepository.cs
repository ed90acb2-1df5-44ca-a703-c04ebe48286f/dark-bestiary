using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkillSetFileRepository : FileRepository<int, SkillSetData, SkillSet>, ISkillSetRepository
    {
        public SkillSetFileRepository(IFileReader reader, SkillSetMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/skill_sets.json";
        }
    }
}