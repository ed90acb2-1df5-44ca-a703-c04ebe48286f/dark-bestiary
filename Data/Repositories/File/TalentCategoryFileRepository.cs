using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Talents;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class TalentCategoryFileRepository
        : FileRepository<int, TalentCategoryData, TalentCategory>, ITalentCategoryRepository
    {
        public TalentCategoryFileRepository(IFileReader loader, TalentCategoryMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/talent_categories.json";
        }
    }
}