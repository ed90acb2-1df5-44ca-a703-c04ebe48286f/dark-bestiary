using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Rewards;

namespace DarkBestiary.Data.Repositories.File
{
    public class RewardFileRepository : FileRepository<int, RewardData, Reward>, IRewardRepository
    {
        public RewardFileRepository(IFileReader reader, RewardMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/rewards.json";
        }
    }
}