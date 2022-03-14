using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.Visions;
using Zenject;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.Rewards
{
    public class LevelupRewardManager : IInitializable
    {
        private readonly List<Reward> pendingRewards = new List<Reward>();
        private readonly IRewardRepository rewardRepository;

        private List<LevelupReward> levelupRewards;
        private Character character;

        public LevelupRewardManager(IRewardRepository rewardRepository)
        {
            this.rewardRepository = rewardRepository;
        }

        public void Initialize()
        {
            this.levelupRewards = this.rewardRepository.FindAll().OfType<LevelupReward>().ToList();
            GameState.AnyGameStateEnter += OnGameStateEnter;

            CharacterManager.CharacterSelected += OnCharacterSelected;
        }

        private void OnGameStateEnter(GameState state)
        {
            if (!state.IsHub || this.pendingRewards.Count == 0 || this.character == null)
            {
                return;
            }

            // Note: Wait for VisionManager to evaluate victory conditions.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                if (VisionManager.Instance?.IsVictoryOrDefeat == true)
                {
                    this.pendingRewards.Clear();
                    return;
                }

                LevelupPopup.Instance.Show(
                    this.character.Entity.GetComponent<ExperienceComponent>().Experience.Level,
                    this.pendingRewards
                );

                this.pendingRewards.Clear();
            });
        }

        private void OnCharacterSelected(Character character)
        {
            this.character = character;

            var experienceComponent = this.character.Entity.GetComponent<ExperienceComponent>();
            experienceComponent.Terminated += OnTerminated;
            experienceComponent.Experience.LevelUp += OnLevelUp;
        }

        private void OnTerminated(Component component)
        {
            var experienceComponent = this.character.Entity.GetComponent<ExperienceComponent>();
            experienceComponent.Terminated -= OnTerminated;
            experienceComponent.Experience.LevelUp -= OnLevelUp;

            this.pendingRewards.Clear();
        }

        private void OnLevelUp(Experience experience)
        {
            var rewards = this.levelupRewards.Where(r => r.Level == experience.Level).ToList();

            if (rewards.Count == 0)
            {
                var reward = new AttributePointsReward(new AttributePointsRewardData {Count = Constants.AttributePointsPerLevel});

                reward.Prepare(this.character.Entity);
                reward.Claim(this.character.Entity);

                this.pendingRewards.Add(reward);
                return;
            }

            foreach (var reward in rewards)
            {
                reward.Prepare(this.character.Entity);
                reward.Claim(this.character.Entity);

                this.pendingRewards.AddRange(reward.Rewards);

                OnGameStateEnter(Game.Instance.State);

                break;
            }
        }
    }
}