using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class RarityFileRepository : FileRepository<int, ItemRarityData, Rarity>, IRarityRepository
    {
        public RarityFileRepository(IFileReader loader, ItemRarityMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/item_rarities.json";
        }
    }
}