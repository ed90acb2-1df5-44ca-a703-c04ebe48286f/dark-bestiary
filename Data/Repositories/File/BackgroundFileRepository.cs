using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class BackgroundFileRepository : FileRepository<int, BackgroundData, Background>, IBackgroundRepository
    {
        public BackgroundFileRepository(IFileReader loader, BackgroundMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/backgrounds.json";
        }
    }
}