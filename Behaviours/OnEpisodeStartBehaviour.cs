using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Scenarios;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class OnEpisodeStartBehaviour : Behaviour
    {
        private readonly EffectBehaviourData data;
        private readonly IEffectRepository effectRepository;

        public OnEpisodeStartBehaviour(EffectBehaviourData data, IEffectRepository effectRepository,
            List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            this.data = data;
            this.effectRepository = effectRepository;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            Episode.AnyEpisodeStarted += OnAnyEpisodeStarted;
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            Episode.AnyEpisodeStarted -= OnAnyEpisodeStarted;
        }

        private void OnAnyEpisodeStarted(Episode episode)
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