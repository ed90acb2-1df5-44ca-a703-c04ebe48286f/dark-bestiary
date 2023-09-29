using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class ChainEffect : Effect
    {
        private readonly ChainEffectData m_Data;
        private readonly BoardNavigator m_BoardNavigator;
        private readonly IEffectRepository m_EffectRepository;
        private readonly List<GameObject> m_Hits;

        private int m_Counter;

        public ChainEffect(ChainEffectData data, List<ValidatorWithPurpose> validators,
            BoardNavigator boardNavigator, IEffectRepository effectRepository) : base(data, validators)
        {
            m_Hits = new List<GameObject>();
            m_Data = data;
            m_BoardNavigator = boardNavigator;
            m_EffectRepository = effectRepository;
        }

        protected override Effect New()
        {
            return new ChainEffect(m_Data, Validators, m_BoardNavigator, m_EffectRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            m_Hits.Add(caster);

            ApplyEffect(caster, caster, target);
        }

        private void ApplyEffect(GameObject previous, GameObject caster, GameObject target)
        {
            m_Hits.Add(target);

            var effect = Inherit(GetEffect());
            effect.Origin = previous;
            effect.Finished += OnEffectFinished;
            effect.Apply(caster, target);
        }

        public Effect GetEffect()
        {
            return m_EffectRepository.Find(m_Data.EffectId);
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;

            m_Counter++;

            if (m_Counter >= m_Data.Times)
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

            var next = m_BoardNavigator
                .EntitiesInRadius(target.transform.position, m_Data.Radius)
                .Where(entity =>
                    Validators.ByPurpose(ValidatorPurpose.Other).Validate(caster, entity) &&
                    !m_Hits.Contains(entity))
                .OrderBy(entity => (entity.transform.position - target.transform.position).sqrMagnitude)
                .FirstOrDefault();

            if (next == null)
            {
                MaybeApplyFinalEffect(effect.Target, effect.Caster, effect.Target);
                return;
            }

            Timer.Instance.Wait(m_Data.Period, () => { ApplyEffect(target, caster, next); });
        }

        private void MaybeApplyFinalEffect(object previous, GameObject caster, object target)
        {
            var finalEffect = m_EffectRepository.Find(m_Data.FinalEffectId);

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