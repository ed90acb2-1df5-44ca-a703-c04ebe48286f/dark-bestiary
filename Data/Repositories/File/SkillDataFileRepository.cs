using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    internal class SkillDataMapper : FakeMapper<SkillData> {}

    public class SkillDataFileRepository : FileRepository<int, SkillData, SkillData>, ISkillDataRepository
    {
        public SkillDataFileRepository(IFileReader reader) : base(reader, new SkillDataMapper())
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/skills.json";
        }
    }
}