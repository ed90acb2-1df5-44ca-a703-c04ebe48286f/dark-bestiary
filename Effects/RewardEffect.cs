using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RewardEffect : Effect
    {
        private readonly RewardEffectData data;
        private readonly IRewardRepository rewardRepository;

        public RewardEffect(RewardEffectData data, List<ValidatorWithPurpose> validators,
            IRewardRepository rewardRepository) : base(data, validators)
        {
            this.data = data;
            this.rewardRepository = rewardRepository;
        }

        protected override Effect New()
        {
            return new RewardEffect(this.data, this.Validators, this.rewardRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var reward = this.rewardRepository.FindOrFail(this.data.RewardId);
            reward.Prepare(target);
            reward.Claim(target);

            TriggerFinished();
        }
    }
}