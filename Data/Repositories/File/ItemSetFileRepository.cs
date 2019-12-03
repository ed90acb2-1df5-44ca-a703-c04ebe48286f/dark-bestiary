using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class ItemSetFileRepository : FileRepository<int, ItemSetData, ItemSet>, IItemSetRepository
    {
        public ItemSetFileRepository(IFileReader loader, ItemSetMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/item_sets.json";
        }
    }
}