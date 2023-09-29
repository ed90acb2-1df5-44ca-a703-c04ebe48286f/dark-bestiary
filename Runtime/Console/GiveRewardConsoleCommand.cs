using DarkBestiary.Data.Repositories;

namespace DarkBestiary.Console
{
    public class GiveRewardConsoleCommand : IConsoleCommand
    {
        private readonly IRewardRepository m_RewardRepository;

        public GiveRewardConsoleCommand(IRewardRepository rewardRepository)
        {
            m_RewardRepository = rewardRepository;
        }

        public string GetSignature()
        {
            return "give_reward";
        }

        public string GetDescription()
        {
            return "Give reward. (Format: [rewardId]";
        }

        public string Execute(string input)
        {
            var rewardId = int.Parse(input.Split()[0]);
            var reward = m_RewardRepository.FindOrFail(rewardId);

            reward.Prepare(Game.Instance.Character.Entity);
            reward.Claim(Game.Instance.Character.Entity);

            return $"{Game.Instance.Character.Name} received reward {reward.GetType().Name}";
        }
    }
}