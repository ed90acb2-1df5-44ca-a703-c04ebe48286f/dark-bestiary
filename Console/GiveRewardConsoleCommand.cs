using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;

namespace DarkBestiary.Console
{
    public class GiveRewardConsoleCommand : IConsoleCommand
    {
        private readonly IRewardRepository rewardRepository;
        private readonly CharacterManager characterManager;

        public GiveRewardConsoleCommand(IRewardRepository rewardRepository, CharacterManager characterManager)
        {
            this.rewardRepository = rewardRepository;
            this.characterManager = characterManager;
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
            var reward = this.rewardRepository.FindOrFail(rewardId);

            reward.Prepare(this.characterManager.Character.Entity);
            reward.Claim(this.characterManager.Character.Entity);

            return $"{this.characterManager.Character.Name} received reward {reward.GetType().Name}";
        }
    }
}