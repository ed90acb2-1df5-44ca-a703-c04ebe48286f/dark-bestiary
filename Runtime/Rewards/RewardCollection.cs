using System;
using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Exceptions;
using UnityEngine;

namespace DarkBestiary.Rewards
{
    public class RewardCollection : Reward
    {
        public List<Reward> Rewards { get; private set; }

        private Reward m_ChosenReward;

        private readonly RewardCollectionData m_Data;
        private readonly IRewardRepository m_RewardRepository;

        public RewardCollection(RewardCollectionData data, IRewardRepository rewardRepository)
        {
            m_Data = data;
            m_RewardRepository = rewardRepository;
        }

        public void Choose(Reward reward)
        {
            if (!Rewards.Contains(reward))
            {
                throw new Exception("Chosen reward is not in collection.");
            }

            m_ChosenReward = reward;
        }

        protected override void OnPrepare(GameObject entity)
        {
            Rewards = m_RewardRepository.Find(m_Data.Rewards);

            foreach (var reward in Rewards)
            {
                reward.Prepare(entity);
            }
        }

        protected override void OnClaim(GameObject entity)
        {
            if (!m_Data.IsChoosable)
            {
                foreach (var reward in Rewards)
                {
                    reward.Claim(entity);
                }

                return;
            }

            if (m_ChosenReward == null)
            {
                throw new MustChooseRewardException();
            }

            m_ChosenReward.Claim(entity);
        }
    }
}