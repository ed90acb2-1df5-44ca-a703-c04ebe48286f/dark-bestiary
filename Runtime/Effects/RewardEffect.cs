using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class RewardEffect : Effect
    {
        private readonly RewardEffectData m_Data;
        private readonly IRewardRepository m_RewardRepository;

        public RewardEffect(RewardEffectData data, List<ValidatorWithPurpose> validators,
            IRewardRepository rewardRepository) : base(data, validators)
        {
            m_Data = data;
            m_RewardRepository = rewardRepository;
        }

        protected override Effect New()
        {
            return new RewardEffect(m_Data, Validators, m_RewardRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            var reward = m_RewardRepository.FindOrFail(m_Data.RewardId);
            reward.Prepare(target);
            reward.Claim(target);

            TriggerFinished();
        }
    }
}