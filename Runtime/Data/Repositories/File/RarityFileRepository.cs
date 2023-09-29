using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Items;

namespace DarkBestiary.Data.Repositories.File
{
    public class RarityFileRepository : FileRepository<int, ItemRarityData, Rarity>, IRarityRepository
    {
        public RarityFileRepository(IFileReader reader, ItemRarityMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/item_rarities.json";
        }
    }
}