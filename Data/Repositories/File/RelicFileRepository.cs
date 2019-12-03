using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class RelicFileRepository : FileRepository<int, RelicData, Relic>, IRelicRepository
    {
        public RelicFileRepository(IFileReader loader, RelicMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/relics.json";
        }
    }
}