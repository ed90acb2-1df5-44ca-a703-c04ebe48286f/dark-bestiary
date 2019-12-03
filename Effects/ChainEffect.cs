using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ChainEffect : Effect
    {
        private readonly ChainEffectData data;
        private readonly List<Validator> validators;
        private readonly BoardNavigator boardNavigator;
        private readonly IEffectRepository effectRepository;
        private readonly List<GameObject> hits;

        private int counter;

        public ChainEffect(ChainEffectData data, List<Validator> validators,
            BoardNavigator boardNavigator, IEffectRepository effectRepository) : base(data, new List<Validator>())
        {
            this.hits = new List<GameObject>();
            this.data = data;
            this.validators = validators;
            this.boardNavigator = boardNavigator;
            this.effectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new ChainEffect(this.data, this.validators, this.boardNavigator, this.effectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            this.hits.Add(caster);

            ApplyEffect(caster, caster, target);
        }

        private void ApplyEffect(GameObject previous, GameObject caster, GameObject target)
        {
            this.hits.Add(target);

            var effect = Inherit(this.effectRepository.Find(this.data.EffectId));
            effect.Origin = previous;
            effect.Finished += OnEffectFinished;
            effect.Apply(caster, target);
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;

            this.counter++;

            if (this.counter >= this.data.Times)
            {
                MaybeApplyFinalEffect(effect.Target, effect.Caster, effect.Target);
                return;
            }

            var target = effect.Target as GameObject;
            var caster = effect.Caster;

            if (target == null)
            {
                MaybeApplyFinalEffect(effect.Target, effect.Caster, effect.Target);
                return;
            }

            var next = this.boardNavigator
                .EntitiesInRadius(target.transform.position, this.data.Radius)
                .Where(entity =>
                    this.validators.All(validator => validator.Validate(caster, entity)) &&
                    !this.hits.Contains(entity))
                .OrderBy(entity => (entity.transform.position - target.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (next == null)
            {
                MaybeApplyFinalEffect(effect.Target, effect.Caster, effect.Target);
                return;
            }

            Timer.Instance.Wait(this.data.Period, () => { ApplyEffect(target, caster, next); });
        }

        private void MaybeApplyFinalEffect(object previous, GameObject caster, object target)
        {
            var finalEffect = this.effectRepository.Find(this.data.FinalEffectId);

            if (finalEffect == null)
            {
                TriggerFinished();
                return;
            }

            finalEffect = Inherit(finalEffect);
            finalEffect.Origin = previous;
            finalEffect.Finished += OnFinalEffectFinished;
            finalEffect.Apply(caster, target);
        }

        private void OnFinalEffectFinished(Effect effect)
        {
            effect.Finished -= OnFinalEffectFinished;
            TriggerFinished();
        }
    }
}