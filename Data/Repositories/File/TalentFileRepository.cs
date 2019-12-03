using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Talents;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class TalentFileRepository : FileRepository<int, TalentData, Talent>, ITalentRepository
    {
        public TalentFileRepository(IFileReader loader, TalentMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/talents.json";
        }
    }
}