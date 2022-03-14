using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Talents;

namespace DarkBestiary.Data.Repositories.File
{
    public class TalentCategoryFileRepository
        : FileRepository<int, TalentCategoryData, TalentCategory>, ITalentCategoryRepository
    {
        public TalentCategoryFileRepository(IFileReader reader, TalentCategoryMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/talent_categories.json";
        }
    }
}