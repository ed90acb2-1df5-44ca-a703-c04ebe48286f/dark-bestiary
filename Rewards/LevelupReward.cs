using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Rewards
{
    public class LevelupReward : RewardCollection
    {
        public int Level { get; }

        public LevelupReward(LevelupRewardData data, IRewardRepository rewardRepository) : base(data, rewardRepository)
        {
            Level = data.Level;
        }
    }
}