using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Scenarios;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnEpisodeCompleteBehaviour : Behaviour
    {
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnEpisodeCompleteBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Episode.AnyEpisodeCompleted += OnAnyEpisodeCompleted;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Episode.AnyEpisodeCompleted -= OnAnyEpisodeCompleted;
        }

        private void OnAnyEpisodeCompleted(Episode episode)
        {
            if (!Target.gameObject.activeSelf)
            {
                return;
            }

            var effect = this.effectRepository.Find(this.data.EffectId);
            effect.StackCount = StackCount;
            effect.Apply(Caster, Caster);
        }
    }
}