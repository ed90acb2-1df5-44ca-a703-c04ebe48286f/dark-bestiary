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

        private Reward chosenReward;

        private readonly RewardCollectionData data;
        private readonly IRewardRepository rewardRepository;

        public RewardCollection(RewardCollectionData data, IRewardRepository rewardRepository)
        {
            this.data = data;
            this.rewardRepository = rewardRepository;
        }

        public void Choose(Reward reward)
        {
            if (!Rewards.Contains(reward))
            {
                throw new Exception("Chosen reward is not in collection.");
            }

            this.chosenReward = reward;
        }

        protected override void OnPrepare(GameObject entity)
        {
            Rewards = this.rewardRepository.Find(this.data.Rewards);

            foreach (var reward in Rewards)
            {
                reward.Prepare(entity);
            }
        }

        protected override void OnClaim(GameObject entity)
        {
            if (!this.data.IsChoosable)
            {
                foreach (var reward in Rewards)
                {
                    reward.Claim(entity);
                }

                return;
            }

            if (this.chosenReward == null)
            {
                throw new MustChooseRewardException();
            }

            this.chosenReward.Claim(entity);
        }
    }
}