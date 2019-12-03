using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Rewards;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class RewardFileRepository : FileRepository<int, RewardData, Reward>, IRewardRepository
    {
        public RewardFileRepository(IFileReader loader, RewardMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/rewards.json";
        }
    }
}