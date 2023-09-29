using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.UI.Elements;
using Zenject;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.Rewards
{
    public class LevelupRewardManager : IInitializable
    {
        private readonly List<Reward> m_PendingRewards = new();
        private readonly IRewardRepository m_RewardRepository;

        private List<LevelupReward> m_LevelupRewards;
        private Character m_Character;

        public LevelupRewardManager(IRewardRepository rewardRepository)
        {
            m_RewardRepository = rewardRepository;
        }

        public void Initialize()
        {
            m_LevelupRewards = m_RewardRepository.FindAll().OfType<LevelupReward>().ToList();

            Game.Instance.SceneSwitched += OnSceneSwitched;
            Game.Instance.CharacterSwitched += OnCharacterSwitched;
        }

        private void OnSceneSwitched()
        {
            if (!Game.Instance.IsTown || m_PendingRewards.Count == 0 || m_Character == null)
            {
                return;
            }

            // Note: Wait for VisionManager to evaluate victory conditions.
            Timer.Instance.WaitForFixedUpdate(() =>
            {
                LevelupPopup.Instance.Show(
                    m_Character.Entity.GetComponent<ExperienceComponent>().Experience.Level,
                    m_PendingRewards
                );

                m_PendingRewards.Clear();
            });
        }

        private void OnCharacterSwitched()
        {
            m_Character = Game.Instance.Character;

            var experienceComponent = m_Character.Entity.GetComponent<ExperienceComponent>();
            experienceComponent.Terminated += OnTerminated;
            experienceComponent.Experience.LevelUp += OnLevelUp;
        }

        private void OnTerminated(Component component)
        {
            var experienceComponent = m_Character.Entity.GetComponent<ExperienceComponent>();
            experienceComponent.Terminated -= OnTerminated;
            experienceComponent.Experience.LevelUp -= OnLevelUp;

            m_PendingRewards.Clear();
        }

        private void OnLevelUp(Experience experience)
        {
            var rewards = m_LevelupRewards.Where(r => r.Level == experience.Level).ToList();

            if (rewards.Count == 0)
            {
                var reward = new AttributePointsReward(new AttributePointsRewardData {Count = Constants.c_AttributePointsPerLevel});

                reward.Prepare(m_Character.Entity);
                reward.Claim(m_Character.Entity);

                m_PendingRewards.Add(reward);
                return;
            }

            foreach (var reward in rewards)
            {
                reward.Prepare(m_Character.Entity);
                reward.Claim(m_Character.Entity);

                m_PendingRewards.AddRange(reward.Rewards);

                OnSceneSwitched();

                break;
            }
        }
    }
}