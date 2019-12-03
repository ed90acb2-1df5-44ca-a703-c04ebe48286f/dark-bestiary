using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class SkinFileRepository : FileRepository<int, SkinData, Skin>, ISkinRepository
    {
        public SkinFileRepository(IFileReader loader, SkinMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/skins.json";
        }
    }
}